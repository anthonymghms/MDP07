package com.example.roadguard

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.TextView
import com.example.roadguard.databinding.ActivityEmailVerificationBinding

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