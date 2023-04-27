package com.example.roadguard

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import androidx.appcompat.app.AppCompatDelegate
import androidx.lifecycle.lifecycleScope
import com.example.roadguard.main.MainActivity
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.launch

class SplashActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        setTheme(R.style.SplashScreenTheme)
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_splash)

        lifecycleScope.launch {

            when (DataStoreHelper.getAppTheme(this@SplashActivity).first()) {
                "dark" -> AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_YES)
                "light" -> AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO)
                else -> AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_FOLLOW_SYSTEM)
            }
        }

        checkKeepMeLoggedIn()
    }

    private fun checkKeepMeLoggedIn() {
        lifecycleScope.launch {
            DataStoreHelper.getKeepMeLoggedIn(this@SplashActivity).collect { keepMeLoggedIn ->
                val intent = if (keepMeLoggedIn) {
                    val token = DataStoreHelper.getToken(this@SplashActivity).first()
                    if (token != null) {
                        Intent(this@SplashActivity, HomeActivity::class.java)
                    } else {
                        Intent(this@SplashActivity, MainActivity::class.java)
                    }
                } else {
                    Intent(this@SplashActivity, MainActivity::class.java)
                }

                Handler(Looper.getMainLooper()).postDelayed({
                    startActivity(intent)
                    finish()
                }, 2000)
            }
        }
    }
}
