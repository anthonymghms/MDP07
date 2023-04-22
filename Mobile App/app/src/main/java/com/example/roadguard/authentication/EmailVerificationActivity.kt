package com.example.roadguard.authentication

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.TextView
import com.example.roadguard.R
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.databinding.ActivityEmailVerificationBinding
import org.json.JSONObject

class EmailVerificationActivity : AppCompatActivity(), ResponseCallback {

    private lateinit var binding: ActivityEmailVerificationBinding
    private lateinit var emailAddress: String
    private lateinit var username: String
    private lateinit var emailParagraph: TextView
    private lateinit var resendTextView: TextView
    private var client: HTTPRequest = HTTPRequest()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityEmailVerificationBinding.inflate(layoutInflater)
        setContentView(binding.root)
        emailAddress = intent.getStringExtra("email").toString()
        username = intent.getStringExtra("username").toString()
        emailParagraph = binding.tvEmailParagraph

        resendTextView = binding.tvEmailResendSuccess

        val verifyEmail = getString(R.string.email_paragraph, emailAddress)
        emailParagraph.text = verifyEmail

        binding.btnResend.setOnClickListener {
            client.get(this,"${client.clientLink}auth/sendconfirmationemail", mapOf("username" to username) ,this)
        }


    }

    override fun onSuccess(response: String) {
        val jsonObject = JSONObject(response)
        jsonObject.keys().forEach {
            when (it) {
                "message" -> {
                    runOnUiThread {
                        resendTextView.text = jsonObject.getString(it)
                        resendTextView.visibility = android.view.View.VISIBLE
                    }
                }
                "error" -> {
                    runOnUiThread{
                        resendTextView.text = jsonObject.getString(it)
                        resendTextView.visibility = android.view.View.VISIBLE
                    }
                }
            }
        }
    }

    override fun onFailure(error: Throwable) {
        error.printStackTrace()
    }
}