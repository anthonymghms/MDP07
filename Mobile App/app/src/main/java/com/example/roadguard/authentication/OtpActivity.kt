package com.example.roadguard.authentication

import android.content.Intent
import android.text.Editable
import android.text.TextWatcher
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.util.Log
import android.widget.EditText
import android.widget.TextView
import com.example.roadguard.HomeActivity
import com.example.roadguard.R
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.sharedPrefs.SharedPrefsHelper
import org.json.JSONObject

class OtpActivity : AppCompatActivity(), ResponseCallback {

    private val otpEditTexts = arrayOfNulls<EditText>(6)
    private var client: HTTPRequest = HTTPRequest()
    private lateinit var tvError: TextView
    private lateinit var username: String

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_otp)

        otpEditTexts[0] = findViewById(R.id.et_otp_1)
        otpEditTexts[1] = findViewById(R.id.et_otp_2)
        otpEditTexts[2] = findViewById(R.id.et_otp_3)
        otpEditTexts[3] = findViewById(R.id.et_otp_4)
        otpEditTexts[4] = findViewById(R.id.et_otp_5)
        otpEditTexts[5] = findViewById(R.id.et_otp_6)



        tvError = findViewById(R.id.tv_otp_error)
        username = intent.getStringExtra("username").toString()

        for (i in otpEditTexts.indices) {
            otpEditTexts[i]?.addTextChangedListener(createTextWatcher(i))
        }
    }

    private fun createTextWatcher(index: Int): TextWatcher {
        return object : TextWatcher {
            override fun beforeTextChanged(s: CharSequence, start: Int, count: Int, after: Int) {}

            override fun onTextChanged(s: CharSequence, start: Int, before: Int, count: Int) {
                if (count == 1 && index < otpEditTexts.size - 1) {
                    otpEditTexts[index + 1]?.requestFocus()
                }

                if (index == otpEditTexts.size - 1 && count == 1) {
                    val otp = StringBuilder()
                    for (editText in otpEditTexts) {
                        otp.append(editText?.text.toString())
                    }
                    Log.d("OTP", "Sent OTP: $otp")
                    Log.d("Username", "Received username: $username")
                    val jsonBody = "{\"username\":\"$username\",\"otp\":\"$otp\"}"
                    client.post(this@OtpActivity,"https://roadguard.azurewebsites.net/api/auth/login-2fa",jsonBody,this@OtpActivity)
                }
            }

            override fun afterTextChanged(s: Editable) {}
        }
    }

    override fun onSuccess(response: String) {
        tvError = findViewById(R.id.tv_otp_error)
        val jsonObject = JSONObject(response)
        Log.d("Response", response)
        jsonObject.keys().forEach {
            when (it) {
                "token" -> {
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

            }
        }
    }

    override fun onFailure(error: Throwable) {
        error.printStackTrace()
    }
}
