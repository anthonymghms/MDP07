import cv2
import mediapipe as mp
import numpy as np
import time
import winsound
from mediapipe.python.solutions.drawing_utils import _normalized_to_pixel_coordinates as denormalize_coordinates
  

def get_mediapipe_app(
    max_num_faces=1,
    refine_landmarks=True,
    min_detection_confidence=0.5,
    min_tracking_confidence=0.5,
):
    """Initialize and return Mediapipe FaceMesh Solution Graph object"""
    face_mesh = mp.solutions.face_mesh.FaceMesh(
        max_num_faces=max_num_faces,
        refine_landmarks=refine_landmarks,
        min_detection_confidence=min_detection_confidence,
        min_tracking_confidence=min_tracking_confidence,
    )

    return face_mesh


def get_ear(landmarks, refer_idxs, frame_width, frame_height):
    """
    Calculate Eye Aspect Ratio for one eye.

    Args:
        landmarks: (list) Detected landmarks list
        refer_idxs: (list) Index positions of the chosen landmarks
                            in order P1, P2, P3, P4, P5, P6
        frame_width: (int) Width of captured frame
        frame_height: (int) Height of captured frame

    Returns:
        ear: (float) Eye aspect ratio
    """
    def distance(point_1, point_2):
        """Calculate l2-norm between two points"""
        dist = sum([(i - j) ** 2 for i, j in zip(point_1, point_2)]) ** 0.5
        return dist
    try:
        # Compute the euclidean distance between the horizontal
        coords_points = []
        for i in refer_idxs:
            lm = landmarks[i]
            coord = denormalize_coordinates(lm.x, lm.y, frame_width, frame_height)
            coords_points.append(coord)

        # Eye landmark (x, y)-coordinates
        P2_P6 = distance(coords_points[1], coords_points[5])
        P3_P5 = distance(coords_points[2], coords_points[4])
        P1_P4 = distance(coords_points[0], coords_points[3])

        # Compute the eye aspect ratio
        ear = (P2_P6 + P3_P5) / (2.0 * P1_P4)

    except:
        ear = 0.0
        coords_points = None

    return ear, coords_points


def calculate_avg_ear(landmarks, left_eye_idxs, right_eye_idxs, image_w, image_h):
    # Calculate Eye aspect ratio

    left_ear, left_lm_coordinates = get_ear(landmarks, left_eye_idxs, image_w, image_h)
    right_ear, right_lm_coordinates = get_ear(landmarks, right_eye_idxs, image_w, image_h)
    Avg_EAR = (left_ear + right_ear) / 2.0

    return Avg_EAR, (left_lm_coordinates, right_lm_coordinates)


def plot_eye_landmarks(frame, left_lm_coordinates, right_lm_coordinates, color):
    for lm_coordinates in [left_lm_coordinates, right_lm_coordinates]:
        if lm_coordinates:
            for coord in lm_coordinates:
                cv2.circle(frame, coord, 2, color, -1)
    return frame


def plot_text(image, text, origin, color, font=cv2.FONT_HERSHEY_SIMPLEX, fntScale=0.8, thickness=2):
    image = cv2.putText(image, text, origin, font, fntScale, color, thickness)
    return image


eye_idxs = {
            "left": [362, 385, 387, 263, 373, 380],
            "right": [33, 160, 158, 133, 153, 144],
        }
state_tracker = {
            "start_time": time.perf_counter(),
            "drowsy_time": 0.0,  # Holds the amount of time passed with EAR < EAR_threshold
            "color": (255,0,0),
            "play_alarm": False,
            "face_direction": "",
        }
thresholds = {
    "EAR_threshold": 0.18,
    "wait_time": 1.0,
}

face_mesh = get_mediapipe_app()
cap = cv2.VideoCapture(0)
alarm_started = False
while cap.isOpened():
    if state_tracker["play_alarm"] and not alarm_started:
        alarm_started = True
        winsound.PlaySound(r'C:\alarm.wav', winsound.SND_ASYNC)
    if not state_tracker["play_alarm"]:
        winsound.PlaySound(None, winsound.SND_PURGE)
        
    success, image = cap.read()
    # Flip the image horizontally for a later e-view display
    # Also convert the color space from BGR to RGB
    image = cv2.cvtColor(cv2.flip(image, 1), cv2.COLOR_BGR2RGB)
    # To improve performance
    image.flags.writeable = False 
    # Get the result
    results = face_mesh.process(image)
    # To improve performance
    image.flags.writeable = True
    # Convert the color space from RGB to BGR
    image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
    img_h, img_w, _ = image.shape
    
    drowsy_time_txt_pos = (10, int(img_h // 2 * 1.7))
    ALM_txt_pos = (10, int(img_h // 2 * 1.85))
    EAR_txt_pos = (500, 200)
    
    face_3d = []
    face_2d = []
    if results.multi_face_landmarks:
        landmarks = results.multi_face_landmarks[0].landmark
        for idx, lm in enumerate(landmarks):
            if idx == 33 or idx == 263 or idx == 1 or idx == 61 or idx == 291 or idx == 199:
                if idx == 1:
                    nose_2d = (lm.x * img_w, lm.y * img_h)
                x, y = int(lm.x * img_w), int(lm.y * img_h)                    
                # Get the 2D Coordinates
                face_2d.append([x, y])
                # Get the 3D Coordinates
                face_3d.append([x, y, lm.z])                
        # Convert it to the NumPy array
        face_2d = np.array(face_2d, dtype=np.float64)
        # Convert it to the NumPy array
        face_3d = np.array(face_3d, dtype=np.float64)
        # The camera matrix
        focal_length = 1 * img_w
        cam_matrix = np.array([[focal_length, 0, img_h / 2],
                               [0, focal_length, img_w / 2],
                               [0, 0, 1]])
        # The distortion parameters
        dist_matrix = np.zeros((4, 1), dtype=np.float64)
        # Solve PnP
        success, rot_vec, _ = cv2.solvePnP(face_3d, face_2d, cam_matrix, dist_matrix)
        # Get rotational matrix
        rmat, _ = cv2.Rodrigues(rot_vec)
        # Get angles
        angles, _, _, _, _, _ = cv2.RQDecomp3x3(rmat)
        # Get the y rotation degree
        x = angles[0] * 360
        y = angles[1] * 360
        z = angles[2] * 360            
        # draw face direction
        p1 = (int(nose_2d[0]), int(nose_2d[1]))
        p2 = (int(nose_2d[0] + y * 10), int(nose_2d[1] - x * 10))
        cv2.line(image, p1, p2, (255, 0, 0), 3)            
        # save direction(left and right can be removed no need for them)
        if x < -10:
            state_tracker["face_direction"] = "Down"
        elif x > 10:
            state_tracker["face_direction"] = "Up"
        elif y < -10:
            state_tracker["face_direction"] = "Left"
        elif y > 10:
            state_tracker["face_direction"] = "Right"
        else:
            state_tracker["face_direction"] = "Forward"                          
        # Add the text on the image
        cv2.putText(image, state_tracker["face_direction"], (20, 50), cv2.FONT_HERSHEY_SIMPLEX, 2, (0, 255, 0), 2)
        cv2.putText(image, "x: " + str(np.round(x, 2)), (500, 50), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)
        cv2.putText(image, "y: " + str(np.round(y, 2)), (500, 100), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)
        cv2.putText(image, "z: " + str(np.round(z, 2)), (500, 150), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 0, 255), 2)
        #END
        EAR, coordinates = calculate_avg_ear(landmarks, eye_idxs["left"], eye_idxs["right"], img_w, img_h)
        if EAR < thresholds["EAR_threshold"] or state_tracker["face_direction"] == "Down":
            # Increase drowsy_time to track the time period with EAR less than the threshold
            # and reset the start_time for the next iteration.
            end_time = time.perf_counter()
            state_tracker["drowsy_time"] += end_time - state_tracker["start_time"]
            state_tracker["start_time"] = end_time
            state_tracker["color"] = (0,0,255)
            if state_tracker["drowsy_time"] >= thresholds["wait_time"]:
                state_tracker["play_alarm"] = True
                plot_text(image, "WAKE UP!", ALM_txt_pos, state_tracker["color"])
            else:
                state_tracker["start_time"] = time.perf_counter()
                state_tracker["color"] = (0,255,0)
                state_tracker["play_alarm"] = False
                alarm_started = False
            EAR_txt = f"EAR: {round(EAR, 2)}"
            drowsy_time_txt = f"DROWSY: {round(state_tracker['drowsy_time'], 3)} Secs"
            plot_text(image, EAR_txt, EAR_txt_pos, state_tracker["color"])
            plot_text(image, drowsy_time_txt, drowsy_time_txt_pos, state_tracker["color"])
        else:
            state_tracker["start_time"] = time.perf_counter()
            state_tracker["drowsy_time"] = 0.0
            state_tracker["color"] = (0,255,0)
            state_tracker["play_alarm"] = False
            alarm_started = False
        #Change the eyePlot color
        if state_tracker["drowsy_time"] < thresholds["wait_time"]:
            image = plot_eye_landmarks(image, coordinates[0], coordinates[1], (0,255,0))
        else:
            image = plot_eye_landmarks(image, coordinates[0], coordinates[1], (0,0,255))

    cv2.imshow('Drowsiness Detection', image)
    if cv2.waitKey(5) & 0xFF == ord('q'):
        break
    
cap.release()
cv2.destroyAllWindows()