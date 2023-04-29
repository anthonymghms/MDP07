package com.example.roadguard.main

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.example.roadguard.HomeActivity
import com.example.roadguard.authentication.LoginActivity
import com.example.roadguard.authentication.RegisterActivity
import com.example.roadguard.databinding.ActivityMainBinding

class MainActivity : AppCompatActivity() {

    private lateinit var binding: ActivityMainBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMainBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.btnLogin.setOnClickListener{
            navigateToLoginActivity()
        }

        binding.btnRegister.setOnClickListener{
            navigateToRegisterActivity()
        }

    }

    private fun navigateToLoginActivity() {
        startActivity(Intent(this, LoginActivity::class.java))
    }

    private fun navigateToRegisterActivity() {
        startActivity(Intent(this, RegisterActivity::class.java))
    }
}
