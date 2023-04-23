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
import com.microsoft.signalr.HubConnectionBuilder
import android.graphics.Color
import android.widget.LinearLayout
import android.widget.Chronometer
import com.microsoft.signalr.HubConnection

class HomeActivity : BaseActivity(),ResponseCallback {

    private lateinit var binding: ActivityHomeBinding

    private val client = HTTPRequest()
    private fun userHasChosenTwoFactorAuth(): Boolean = true
    private var twoFactorAuthDialog: AlertDialog? = null
    private var notificationDialog: AlertDialog? = null
    private var locationDialog: AlertDialog? = null
    private val hubConnection: HubConnection = HubConnectionBuilder.create("${client.clientAddress}detectionHub").build()
    private lateinit var timer: Chronometer

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
        val token = SharedPrefsHelper.getToken(this)
        client.get(this, "${client.clientLink}python/startdetection",null,this,token)
    }
    private fun updateDrowsinessAlertViewBackground(color: Int) {
        val drowsinessAlertView = findViewById<LinearLayout>(R.id.drowsiness_alert_view)
        drowsinessAlertView.setBackgroundColor(color)
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)

        findViewById<AppCompatButton>(R.id.start_button).setOnClickListener {
            startScript()
        }

        timer = findViewById(R.id.drowsiness_timer)

        hubConnection.on("DetectionResult", { message ->
            Log.d("DetectionResult", message)
            println(message)
            if (message.trim() == "Asleep") {
                runOnUiThread {
                    timer.stop()
                    timer.setTextColor(Color.RED)
                    updateDrowsinessAlertViewBackground(Color.RED)
                    binding.timerTextView.text = "Asleep"
                }
            }
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
        val jsonResponse = JSONObject(response)
        println(jsonResponse)
        if (jsonResponse.getString("message") == "Started detecting"){
            runOnUiThread {
                timer.start()
                updateDrowsinessAlertViewBackground(Color.GREEN)
                binding.timerTextView.text = "Awake"
            }
        }

    }


    override fun onFailure(error: Throwable) {
        error.printStackTrace()
    }
}
