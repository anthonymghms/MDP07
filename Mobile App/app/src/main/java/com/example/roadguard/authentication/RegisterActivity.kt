package com.example.roadguard.authentication

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.TextView
import com.example.roadguard.R
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.databinding.ActivityRegisterBinding
import org.json.JSONObject
import com.example.roadguard.client.HTTPRequest as HTTPRequest


class RegisterActivity : AppCompatActivity(), ResponseCallback {

    private lateinit var binding: ActivityRegisterBinding
    private lateinit var emailAddress: String
    private lateinit var password: String
    private lateinit var confirmPassword: String
    private lateinit var firstName: String
    private lateinit var lastName: String
    private lateinit var phoneNumber: String
    private lateinit var username: String
    private lateinit var tvError: TextView

    private val passwordsNoMatch = "Passwords do not match"
    private val usernameExists = "Username already exists"

    private val client = HTTPRequest()
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityRegisterBinding.inflate(layoutInflater)
        setContentView(binding.root)

        tvError = findViewById(R.id.tv_error)
        tvError.visibility = android.view.View.GONE

        binding.btnRegister.setOnClickListener {
            emailAddress = binding.etEmail.text.toString().trim()
            password = binding.etPassword.text.toString().trim()
            confirmPassword = binding.etConfirmPassword.text.toString().trim()
            firstName = binding.etFirstName.text.toString().trim()
            lastName = binding.etLastName.text.toString().trim()
            phoneNumber = binding.etPhone.text.toString().trim()
            username = binding.etUsername.text.toString().trim()
            when {
                password != confirmPassword -> {
                    tvError.visibility = android.view.View.VISIBLE
                    tvError.text = passwordsNoMatch
                }
                else -> {
                    val jsonRegisterRequest = "{\"FirstName\":\"$firstName\",\"LastName\":\"$lastName\",\"Username\":\"$username\",\"Password\":\"$password\",\"PhoneNumber\":\"$phoneNumber\",\"Email\":\"$emailAddress\"}"
                    client.post(this,"${client.clientLink}auth/register", jsonRegisterRequest, this, mapOf("role" to "User"))
                }
            }
        }
        binding.tvHaveAccount.setOnClickListener {
            startActivity(Intent(this, LoginActivity::class.java))
        }
    }

    override fun onSuccess(response: String) {
        val jsonObject = JSONObject(response)
        jsonObject.keys().forEach {
            when (it) {
                "message" -> {
                    when {
                        jsonObject.getString(it) == "Failed : DuplicateUserName" -> {
                            runOnUiThread {
                                tvError.visibility = android.view.View.VISIBLE
                                tvError.text = usernameExists
                            }
                        }
                        else -> {
                            runOnUiThread {
                                tvError.visibility = android.view.View.GONE
                            }
                            val intent = Intent(this, EmailVerificationActivity::class.java)
                            intent.putExtra("email", emailAddress)
                            intent.putExtra("username", username)
                            startActivity(intent)
                        }
                    }
                }
                "errors" -> {
                    runOnUiThread {
                        tvError.visibility = android.view.View.VISIBLE
                        JSONObject(jsonObject.getString(it)).keys().forEach { x ->
                            when (x) {
                                "Username" -> {
                                    tvError.text = JSONObject(jsonObject.getString(it)).getString(x).replace("[\"","").replace("\"]","")
                                }
                                "Password" -> {
                                    tvError.text = JSONObject(jsonObject.getString(it)).getString(x).replace("[\"","").replace("\"]","")
                                }
                                "Email" -> {
                                    tvError.text = JSONObject(jsonObject.getString(it)).getString(x).replace("[\"","").replace("\"]","").split("\",\"")[0]
                                }
                                "PhoneNumber" -> {
                                    tvError.text = JSONObject(jsonObject.getString(it)).getString(x).replace("[\"","").replace("\"]","")
                                }
                                "FirstName" -> {
                                    tvError.text = JSONObject(jsonObject.getString(it)).getString(x).replace("[\"","").replace("\"]","")
                                }
                                "LastName" -> {
                                    tvError.text = JSONObject(jsonObject.getString(it)).getString(x).replace("[\"","").replace("\"]","")
                                }
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




