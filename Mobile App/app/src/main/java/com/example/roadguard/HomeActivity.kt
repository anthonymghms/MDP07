package com.example.roadguard

import android.app.NotificationChannel
import android.app.NotificationManager
import android.os.Build
import android.os.Bundle
import android.util.Log
import androidx.appcompat.app.AlertDialog
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.databinding.ActivityHomeBinding
import com.example.roadguard.sharedPrefs.SharedPrefsHelper
import org.json.JSONObject


class HomeActivity : BaseActivity(),ResponseCallback {

    private lateinit var binding: ActivityHomeBinding
    private fun userHasChosenTwoFactorAuth(): Boolean = true
    private var twoFactorAuthDialog: AlertDialog? = null
    private var notificationDialog: AlertDialog? = null

    private fun createNotificationChannel() {
        val name: CharSequence = "RoadguardChannel"
        val description = "Channel for Roadguard notifications"
        val importance = NotificationManager.IMPORTANCE_DEFAULT
        val channel = NotificationChannel("Roadguard_id", name, importance)
        channel.description = description
        val notificationManager = getSystemService(
            NotificationManager::class.java
        )
        notificationManager.createNotificationChannel(channel)
    }


    private fun promptNotifications() {
      val builder =  AlertDialog.Builder(this)
            .setTitle("Notifications")
            .setMessage("Would you like to enable notifications?")
            .setPositiveButton("Allow") {_, _ ->
                val userSettingsManager = UserSettingsManager(this)
                userSettingsManager.updateNotificationSettingOnServer(true,this).let{ response ->
                    Log.d("HomeActivity", response.toString())
                }
            }
            .setNegativeButton("No, thank you") {_, _ ->
                val userSettingsManager = UserSettingsManager(this)
                userSettingsManager.updateNotificationSettingOnServer(false,this)
            }
      notificationDialog = builder.create()
        notificationDialog?.show()
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)
        initSlideUpMenu()
        setOutsideTouchListener(R.id.home_activity)
        promptNotifications()
        if (!userHasChosenTwoFactorAuth()) {
            showTwoFactorAuthDialog()
        }
        createNotificationChannel()
    }

    private fun showTwoFactorAuthDialog() {
        val builder = AlertDialog.Builder(this, R.style.AlertDialogCustom)
        val inflater = layoutInflater
        val dialogView = inflater.inflate(R.layout.dialog_two_factor_auth, null)

        builder.setView(dialogView)
        builder.setPositiveButton("Enable") { dialog, _ ->
            dialog.dismiss()
        }

        builder.setNegativeButton("No, thank you") { dialog, _ ->
            dialog.dismiss()
        }

        twoFactorAuthDialog = builder.create()
        twoFactorAuthDialog?.show()
    }





    override fun onDestroy() {
        twoFactorAuthDialog?.dismiss()
        notificationDialog?.dismiss()
        super.onDestroy()
    }

    override fun onSuccess(response: String) {
        val responseJson = JSONObject(response)
        println(responseJson)
    }

    override fun onFailure(error: Throwable) {
        error.printStackTrace()
    }
}