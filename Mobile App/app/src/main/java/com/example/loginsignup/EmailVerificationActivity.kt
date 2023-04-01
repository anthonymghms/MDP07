package com.example.loginsignup

import android.content.ActivityNotFoundException
import android.content.ComponentName
import android.content.Intent
import android.net.Uri
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.TextView
import android.widget.Toast
import com.example.loginsignup.databinding.ActivityEmailVerificationBinding

class EmailVerificationActivity : AppCompatActivity() {

    private lateinit var binding: ActivityEmailVerificationBinding
    private lateinit var emailAddress: String
    private lateinit var emailParagraph: TextView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityEmailVerificationBinding.inflate(layoutInflater)
        setContentView(binding.root)
        emailAddress = intent.getStringExtra("email").toString()
        emailParagraph = binding.tvEmailParagraph

        val verifyEmail = getString(R.string.email_paragraph, emailAddress)
        emailParagraph.text = verifyEmail



    }
}