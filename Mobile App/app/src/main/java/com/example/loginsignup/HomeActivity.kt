package com.example.loginsignup

import android.content.Intent
import android.content.pm.PackageManager
import android.graphics.Color
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.Toast
import androidx.activity.result.contract.ActivityResultContracts
import androidx.camera.core.CameraSelector
import androidx.camera.core.Preview
import androidx.camera.lifecycle.ProcessCameraProvider
import androidx.core.content.ContextCompat
import com.example.loginsignup.databinding.ActivityHomeBinding
import com.example.loginsignup.databinding.ActivityRegisterBinding
import java.util.concurrent.Executor
import java.util.concurrent.ExecutorService
import java.util.concurrent.Executors


class HomeActivity : AppCompatActivity() {

    private lateinit var binding: ActivityHomeBinding
    private lateinit var cameraExecutor: ExecutorService



    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)
        binding.previewView.visibility = View.GONE
        cameraExecutor = Executors.newSingleThreadExecutor()
        requestPermission()

        binding.homeBtnStop.setOnClickListener {
            stopCamera()
        }

        binding.btnLogout.setOnClickListener {
            startActivity(Intent(this, LoginActivity::class.java))
        }
    }

    private fun requestPermission() {
        binding.homeBtnStart.setOnClickListener {
            requestCameraPermissionIfMissing { granted ->
                if (granted)
                    startCamera()
                else
                    Toast.makeText(this,"Please Allow The Permission",Toast.LENGTH_SHORT).show()

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

    fun buildPreviewUseCase(): Preview {
        return Preview.Builder().build().also {it.setSurfaceProvider(binding.previewView.surfaceProvider) }
    }
}