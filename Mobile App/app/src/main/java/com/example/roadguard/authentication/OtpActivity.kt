package com.example.roadguard.authentication

import android.text.Editable
import android.text.TextWatcher
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.widget.EditText
import android.widget.TextView
import com.example.roadguard.R
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback

class OtpActivity : AppCompatActivity(), ResponseCallback {

    private val otpEditTexts = arrayOfNulls<EditText>(6)
    private var client: HTTPRequest = HTTPRequest()
    private lateinit var tvError: TextView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_otp)

        otpEditTexts[0] = findViewById(R.id.et_otp_1)
        otpEditTexts[1] = findViewById(R.id.et_otp_2)
        otpEditTexts[2] = findViewById(R.id.et_otp_3)
        otpEditTexts[3] = findViewById(R.id.et_otp_4)
        otpEditTexts[4] = findViewById(R.id.et_otp_5)
        otpEditTexts[5] = findViewById(R.id.et_otp_6)

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
                    submitOtp()
                }
            }

            override fun afterTextChanged(s: Editable) {}
        }
    }

    private fun submitOtp() {
        val otp = StringBuilder()
        for (editText in otpEditTexts) {
            otp.append(editText?.text.toString())
        }
    }

    override fun onSuccess(response: String) {
        TODO("Not yet implemented")
    }

    override fun onFailure(error: Throwable) {
        TODO("Not yet implemented")
    }
}
