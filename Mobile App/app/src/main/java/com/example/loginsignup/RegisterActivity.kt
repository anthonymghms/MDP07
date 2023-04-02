package com.example.loginsignup

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.example.loginsignup.databinding.ActivityRegisterBinding
import com.google.android.material.textfield.TextInputEditText
import com.example.loginsignup.HTTPRequest as HTTPRequest


class RegisterActivity : AppCompatActivity() {

    private lateinit var binding: ActivityRegisterBinding
    private lateinit var emailAddress: TextInputEditText
    private val client = HTTPRequest()
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityRegisterBinding.inflate(layoutInflater)
        setContentView(binding.root)

        emailAddress = binding.etEmail

        binding.btnRegister.setOnClickListener {
            client.get("https://192.168.1.114:5001/api/auth/test")
//            val intent = Intent(this, EmailVerificationActivity::class.java)
//            intent.putExtra("email", emailAddress.text.toString())
//            startActivity(intent)
        }

        binding.tvHaveAccount.setOnClickListener {
            startActivity(Intent(this, LoginActivity::class.java))
        }

    }




    }




