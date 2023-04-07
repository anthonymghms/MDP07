package com.example.roadguard.authentication

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.TextView
import com.example.roadguard.*
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.sharedPrefs.SharedPrefsHelper
import com.example.roadguard.databinding.ActivityLoginBinding
import org.json.JSONObject

class LoginActivity : AppCompatActivity(), ResponseCallback {

    private lateinit var binding: ActivityLoginBinding
    private var client: HTTPRequest = HTTPRequest()
    private lateinit var tvError: TextView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityLoginBinding.inflate(layoutInflater)
        setContentView(binding.root)

        tvError = findViewById(R.id.tv_email_error)
        tvError.visibility = android.view.View.GONE

        binding.btnLogin.setOnClickListener {
            val username = binding.etEmail.text.toString()
            val password = binding.etPassword.text.toString()
            val jsonLoginRequest = "{\"username\":\"$username\",\"password\":\"$password\"}"
            client.post("https://roadguard.azurewebsites.net/api/auth/login",jsonLoginRequest, this)
        }


        binding.tvHaventAccount.setOnClickListener {
            startActivity(Intent(this, RegisterActivity::class.java))
        }
    }

    override fun onSuccess(response: String) {
        val jsonObject = JSONObject(response)
        jsonObject.keys().forEach {
            when (it) {
                "token" -> {
                    runOnUiThread {
                        tvError.visibility = android.view.View.GONE
                    }
                    val token = jsonObject.getString(it)
                    SharedPrefsHelper.saveToken(this, token)
                    val intent = Intent(this, HomeActivity::class.java)
                    startActivity(intent)
                }
                "message" -> {
                    runOnUiThread {
                        tvError.visibility = android.view.View.VISIBLE
                        tvError.text = jsonObject.getString(it)
                    }
                }
                "errors" -> {
                    runOnUiThread {
                        tvError.visibility = android.view.View.VISIBLE
                        JSONObject(jsonObject.getString(it)).keys().forEach {x->
                            if (x == "Username") {
                                tvError.text = JSONObject(jsonObject.getString(it)).getString(x).replace("[\"","").replace("\"]","")
                            } else if (x == "Password") {
                                tvError.text = JSONObject(jsonObject.getString(it)).getString(x).replace("[\"","").replace("\"]","")
                            }
                        }
                    }
                }
            }
        }
    }

    override fun onFailure(error: Throwable) {
        error.printStackTrace()
    }
}