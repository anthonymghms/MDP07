import sys

import cv2
import mediapipe as mp
import numpy as np
import time
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

eye_idxs = {
    "left": [362, 385, 387, 263, 373, 380],
    "right": [33, 160, 158, 133, 153, 144],
}
state_tracker = {
    "start_time": time.perf_counter(),
    "drowsy_time": 0.0,  # Holds the amount of time passed with EAR < EAR_threshold
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
        # save direction(left and right can be removed no need for them)
        if x < -13:
            state_tracker["face_direction"] = "Down"
        elif x > 13:
            state_tracker["face_direction"] = "Up"
        else:
            state_tracker["face_direction"] = "Forward"

        EAR, coordinates = calculate_avg_ear(landmarks, eye_idxs["left"], eye_idxs["right"], img_w, img_h)
        if EAR < thresholds["EAR_threshold"] or state_tracker["face_direction"] == "Down":
            # Increase drowsy_time to track the time period with EAR less than the threshold
            # and reset the start_time for the next iteration.
            end_time = time.perf_counter()
            state_tracker["drowsy_time"] += end_time - state_tracker["start_time"]
            state_tracker["start_time"] = end_time
            if state_tracker["drowsy_time"] >= thresholds["wait_time"]:
                # return whether the driver is asleep or not
                print("Asleep")
                break
            else:
                state_tracker["start_time"] = time.perf_counter()
        else:
            state_tracker["start_time"] = time.perf_counter()
            state_tracker["drowsy_time"] = 0.0
    cv2.imshow('Drowsiness Detection', image)
    if cv2.waitKey(5) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
