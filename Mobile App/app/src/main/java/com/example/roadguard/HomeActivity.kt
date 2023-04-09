package com.example.roadguard

import android.content.Intent
import android.content.pm.PackageManager
import android.graphics.Color
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.Toast
import androidx.activity.result.contract.ActivityResultContracts
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.widget.AppCompatButton
import androidx.camera.core.CameraSelector
import androidx.camera.core.Preview
import androidx.camera.lifecycle.ProcessCameraProvider
import androidx.core.content.ContextCompat
import com.example.roadguard.authentication.LoginActivity
import com.example.roadguard.authentication.OtpActivity
import com.example.roadguard.databinding.ActivityHomeBinding
import java.util.concurrent.ExecutorService
import java.util.concurrent.Executors


class HomeActivity : AppCompatActivity() {

    private lateinit var binding: ActivityHomeBinding
    private lateinit var cameraExecutor: ExecutorService
    private var started:Boolean = false
    private fun userHasChosenTwoFactorAuth(): Boolean = false




    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)
        cameraExecutor = Executors.newSingleThreadExecutor()
        requestPermission()

        if (!userHasChosenTwoFactorAuth()) {
            showTwoFactorAuthDialog()
        }

        binding.btnLogout.setOnClickListener {
            startActivity(Intent(this, LoginActivity::class.java))
        }
        binding.btnMap.setOnClickListener {
            startActivity(Intent(this, MapViewActivity::class.java))
        }
        binding.btnOtp.setOnClickListener {
            startActivity(Intent(this, OtpActivity::class.java))
        }
    }

    private fun requestPermission() {
        val homeBtn = findViewById<AppCompatButton>(R.id.home_btn)
        homeBtn.setOnClickListener {
            if (!started) {
                requestCameraPermissionIfMissing { granted ->
                    if (granted)
                        startCamera()
                    else
                        Toast.makeText(this, "Please Allow The Permission", Toast.LENGTH_SHORT).show()

                }
                binding.previewView.visibility = View.VISIBLE
                started = true
                homeBtn.setText(R.string.home_stop_camera)
            } else {
                stopCamera()
                started = false
                homeBtn.setText(R.string.home_start_camera)
            }

        }
    }

    private fun requestCameraPermissionIfMissing(onResult: ((Boolean) -> Unit)) {
        if (ContextCompat.checkSelfPermission(this, android.Manifest.permission.CAMERA ) == PackageManager.PERMISSION_GRANTED)
            onResult(true)
        else
            registerForActivityResult(ActivityResultContracts.RequestPermission()) {
                onResult(it)
            }.launch(android.Manifest.permission.CAMERA)
    }

    private fun startCamera(){
        val processCameraProvider = ProcessCameraProvider.getInstance(this)
        binding.previewView.visibility = View.VISIBLE
        processCameraProvider.addListener({
            try {
                val cameraProvider = processCameraProvider.get()
                val previewUseCase = buildPreviewUseCase()

                cameraProvider.unbindAll()
                cameraProvider.bindToLifecycle(this, CameraSelector.DEFAULT_FRONT_CAMERA, previewUseCase)
            } catch (e: Exception) {
                Log.d("ERROR",e.message.toString())
            }
        }, ContextCompat.getMainExecutor(this))

    }

    private fun stopCamera() {
        try {
            val cameraProvider = ProcessCameraProvider.getInstance(this).get()
            val previewUseCase = buildPreviewUseCase()

            previewUseCase.setSurfaceProvider(null)
            binding.previewView.setBackgroundColor(Color.BLACK)
            binding.previewView.visibility = View.GONE
            cameraProvider.unbindAll()
        } catch (e: Exception) {
            Toast.makeText(this, "Error stopping the camera", Toast.LENGTH_SHORT).show()
        }

    }

    private fun buildPreviewUseCase(): Preview {
        return Preview.Builder().build().also {it.setSurfaceProvider(binding.previewView.surfaceProvider) }
    }

    private fun showTwoFactorAuthDialog() {
        val builder = AlertDialog.Builder(this, R.style.AlertDialogCustom)
        val inflater = layoutInflater
        val dialogView = inflater.inflate(R.layout.dialog_two_factor_auth, null)

        builder.setView(dialogView)
        builder.setPositiveButton("Enable") { dialog, _ ->
            dialog.dismiss()
        }

        builder.setNegativeButton("No, thank you") { dialog, _ ->
            dialog.dismiss()
        }

        val alertDialog = builder.create()
        alertDialog.show()
    }


}