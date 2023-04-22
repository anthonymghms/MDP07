package com.example.roadguard

import android.app.NotificationChannel
import android.app.NotificationManager
import android.os.Bundle
import android.util.Log
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.widget.AppCompatButton
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.databinding.ActivityHomeBinding
import com.example.roadguard.sharedPrefs.SharedPrefsHelper
import org.json.JSONObject
import org.json.JSONException
import com.microsoft.signalr.HubConnectionBuilder


class HomeActivity : BaseActivity(),ResponseCallback {

    private lateinit var binding: ActivityHomeBinding
    private fun userHasChosenTwoFactorAuth(): Boolean = true
    private var twoFactorAuthDialog: AlertDialog? = null
    private var notificationDialog: AlertDialog? = null
    private var locationDialog: AlertDialog? = null
    val hubConnection = HubConnectionBuilder.create("https://roadguard.azurewebsites.net/detectionHub").build()

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
                SharedPrefsHelper.saveNotificationSettings(this, true)
                val userSettingsManager = UserSettingsManager(this)
                userSettingsManager.updatePromptSettingOnServer(this)
            }
            .setNegativeButton("No, thank you") {_, _ ->
                SharedPrefsHelper.saveNotificationSettings(this, false)
                val userSettingsManager = UserSettingsManager(this)
                userSettingsManager.updatePromptSettingOnServer(this)
            }
        notificationDialog = builder.create()
        notificationDialog?.show()
    }

    private fun promptLocation() {
        val builder = AlertDialog.Builder(this)
            .setTitle("Location")
            .setMessage("Would you like to enable location?")
            .setPositiveButton("Allow") {_, _ ->
                SharedPrefsHelper.saveLocationSharing(this, true)
                val userSettingsManager = UserSettingsManager(this)
                userSettingsManager.updatePromptSettingOnServer(this)
            }
            .setNegativeButton("No, thank you") {_, _ ->
                SharedPrefsHelper.saveLocationSharing(this, false)
                val userSettingsManager = UserSettingsManager(this)
                userSettingsManager.updatePromptSettingOnServer(this)
            }

        locationDialog = builder.create()
        locationDialog?.show()
    }

    private fun startScript(){
        val client = HTTPRequest()
        client.post(this, "${client.clientLink}/python/startdetection","",this,null)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)

        findViewById<AppCompatButton>(R.id.start_button).setOnClickListener {
            startScript()
        }

        hubConnection.on("DetectionResult", { message ->
            Log.d("DetectionResult", message)
        }, String::class.java)
        hubConnection.start().blockingAwait()

        initSlideUpMenu()
        setOutsideTouchListener(R.id.home_activity)
        if(SharedPrefsHelper.getLoginCount(this) == 0) {
            promptLocation()
            promptNotifications()
        }
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
        hubConnection.stop()
        twoFactorAuthDialog?.dismiss()
        notificationDialog?.dismiss()
        locationDialog?.dismiss()
        super.onDestroy()
    }

    override fun onSuccess(response: String) {
        println(JSONObject(response))
    }


    override fun onFailure(error: Throwable) {
        error.printStackTrace()
    }
}
