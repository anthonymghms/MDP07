package com.example.roadguard.authentication

import android.content.Intent
import android.net.Uri
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.Toast
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.databinding.ActivityForgotPasswordBinding

class ForgotPasswordActivity : AppCompatActivity(), ResponseCallback {

    private lateinit var binding: ActivityForgotPasswordBinding
    private var client = HTTPRequest()
    private var uri: Uri? = null


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityForgotPasswordBinding.inflate(layoutInflater)
        setContentView(binding.root)

        uri = intent.data;

        if (uri!=null) {
            resetPassword(uri!!)
        }

        binding.btnSubmit.setOnClickListener {
            val newPassword = binding.etNewPassword.text.toString().trim()
            val confirmPassword = binding.etConfirmPassword.text.toString().trim()

            if (newPassword.isNotEmpty() && confirmPassword.isNotEmpty()) {
                if (newPassword == confirmPassword) {
                    startActivity(Intent(this, LoginActivity::class.java))
                    Toast.makeText(this, "Password reset successfully.", Toast.LENGTH_LONG).show()
                    finish()
                } else {
                    Toast.makeText(this, "Passwords do not match.", Toast.LENGTH_LONG).show()
                }
            } else {
                Toast.makeText(this, "Please fill in all fields.", Toast.LENGTH_LONG).show()
            }
        }
    }

    private fun resetPassword(uri: Uri){
        val token = uri.getQueryParameter("token")
        val email = uri.getQueryParameter("email")
        val url = "${client.clientLink}auth/resetpassword"
        val queryParams = mutableMapOf<String, String>()
        token?.let { queryParams["token"] = it }
        email?.let { queryParams["email"] = it }

        client.post(this,url,"{}" ,this,queryParams )

    }

    override fun onSuccess(response: String) {
        TODO("Not yet implemented")
    }

    override fun onFailure(error: Throwable) {
        error.printStackTrace()
    }
}
