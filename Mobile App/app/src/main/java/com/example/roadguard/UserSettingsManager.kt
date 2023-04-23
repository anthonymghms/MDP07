package com.example.roadguard

import android.content.Context
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.sharedPrefs.SharedPrefsHelper
import org.json.JSONObject

class UserSettingsManager(private val context: Context) {

    fun updatePromptSettingOnServer(responseCallback: ResponseCallback) {
        val settingsJson = JSONObject()
        settingsJson.put("notificationsEnabled", SharedPrefsHelper.getNotificationSettings(context))

        val token = SharedPrefsHelper.getToken(context).toString()
        val client = HTTPRequest()

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