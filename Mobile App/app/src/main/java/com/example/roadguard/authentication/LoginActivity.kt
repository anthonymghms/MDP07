package com.example.roadguard.authentication

import DataStoreHelper
import android.content.Intent
import android.net.Uri
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.TextView
import com.example.roadguard.*
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.databinding.ActivityLoginBinding
import org.json.JSONObject
import android.widget.CheckBox
import androidx.lifecycle.lifecycleScope
import kotlinx.coroutines.launch

class LoginActivity : AppCompatActivity(), ResponseCallback {

    private lateinit var binding: ActivityLoginBinding
    private val client: HTTPRequest = HTTPRequest()
    private lateinit var tvError: TextView
    private var uri: Uri? = null
    private lateinit var cbKeepMeLoggedIn: CheckBox

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityLoginBinding.inflate(layoutInflater)
        setContentView(binding.root)

        uri = intent.data

        uri?.let { confirmEmail(it) }

        tvError = binding.tvEmailError
        tvError.visibility = android.view.View.GONE

        cbKeepMeLoggedIn = binding.cbKeepMeLoggedIn

        binding.btnLogin.setOnClickListener {
            performLogin()
        }

        binding.tvHaventAccount.setOnClickListener {
            navigateToRegisterActivity()
        }
    }

    private fun performLogin() {
        val username = binding.etEmail.text.toString().trim()
        val password = binding.etPassword.text.toString().trim()
        val jsonLoginRequest = "{\"username\":\"$username\",\"password\":\"$password\"}"
        client.post(this,"${client.clientLink}auth/login",jsonLoginRequest, this)
    }

    private fun navigateToRegisterActivity() {
        startActivity(Intent(this, RegisterActivity::class.java))
    }

    override fun onSuccess(response: String) {
        val jsonObject = JSONObject(response)
        Log.d("LoginActivity", jsonObject.toString())
        jsonObject.keys().forEach {
            when (it) {
                "loginCount" -> {
                    val loginCount = jsonObject.getInt(it)
                    lifecycleScope.launch{
                        DataStoreHelper.saveLoginCount(this@LoginActivity, loginCount)
                    }
                }
                "token" -> {
                    val token = jsonObject.getString(it)
                    lifecycleScope.launch{
                        DataStoreHelper.saveToken(this@LoginActivity, token)
                        DataStoreHelper.saveKeepMeLoggedIn(this@LoginActivity, cbKeepMeLoggedIn.isChecked)
                    }
                    val intent = Intent(this, HomeActivity::class.java)
                    startActivity(intent)
                }
                "message" -> {
                    when {
                        jsonObject.getString(it).startsWith("Please confirm your email", false) -> {
                            runOnUiThread {
                                val intent = Intent(this, EmailVerificationActivity::class.java)
                                intent.putExtra("email",jsonObject.getString(it).split(" ")[10])
                                intent.putExtra("username", binding.etEmail.text.toString().trim())
                                startActivity(intent)
                            }
                        }
                        jsonObject.getString(it).startsWith("Email",false) -> {
                            runOnUiThread {
                                tvError.visibility = android.view.View.VISIBLE
                                tvError.text = jsonObject.getString(it)
                                tvError.setTextColor(getColor(R.color.green))
                            }
                        }
                        jsonObject.getString(it).startsWith("We have sent an OTP to your Email",false) -> {
                            runOnUiThread {
                                val intent = Intent(this, OtpActivity::class.java)
                                intent.putExtra("username", binding.etEmail.text.toString().trim())
                                startActivity(intent)
                            }
                        }
                        else -> {
                            runOnUiThread {
                                tvError.visibility = android.view.View.VISIBLE
                                tvError.text = jsonObject.getString(it)
                                tvError.setTextColor(getColor(R.color.red))
                            }
                        }
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

    private fun confirmEmail(uri: Uri) {
        val token = uri.getQueryParameter("token")
        val email = uri.getQueryParameter("email")
        val url = "${client.clientLink}auth/ConfirmEmail"
        val queryParams = mutableMapOf<String, String>()
        token?.let { queryParams["token"] = it }
        email?.let { queryParams["email"] = it }

        client.get(this,url, queryParams, this)
    }

    override fun onFailure(error: Throwable) {
        error.printStackTrace()
    }


}