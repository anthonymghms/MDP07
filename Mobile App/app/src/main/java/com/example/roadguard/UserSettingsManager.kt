package com.example.roadguard

import DataStoreHelper
import android.content.Context
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.lifecycleScope
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import kotlinx.coroutines.flow.collect
import kotlinx.coroutines.launch
import org.json.JSONObject

class UserSettingsManager(private val context: Context): AppCompatActivity() {

    fun updatePromptSettingOnServer(responseCallback: ResponseCallback) {
        val settingsJson = JSONObject()
        val client = HTTPRequest()

        lifecycleScope.launch {
            DataStoreHelper.getNotificationSettings(context).collect{notificationSettings ->
                settingsJson.put("notificationsEnabled", notificationSettings)
            }

            DataStoreHelper.getToken(context).collect{token ->
                client.post(
                    context,
                    "${client.clientLink}user/updatesettings",
                    settingsJson.toString(),
                    responseCallback,
                    null,
                    token
                )
            }
        }

    }

}