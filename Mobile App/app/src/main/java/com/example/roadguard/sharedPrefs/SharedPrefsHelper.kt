package com.example.roadguard.sharedPrefs

import android.content.Context

object SharedPrefsHelper {
    private const val PREFERENCES_NAME = "RoadGuardPrefs"
    private const val TOKEN_KEY = "AuthToken"

    fun saveToken(context: Context, token: String) {
        val sharedPreferences = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE)
        sharedPreferences.edit().putString(TOKEN_KEY, token).apply()
    }

    fun getToken(context: Context): String? {
        val sharedPreferences = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE)
        return sharedPreferences.getString(TOKEN_KEY, null)
    }

    fun clearToken(context: Context) {
        val sharedPreferences = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE)
        sharedPreferences.edit().remove(TOKEN_KEY).apply()
    }

    fun saveNotificationSettings(context: Context, enabled: Boolean){
        val sharedPreferences = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE)
        sharedPreferences.edit().putBoolean("notificationsEnabled", enabled).apply()
    }


    fun getNotificationSettings(context: Context): Boolean{
        val sharedPreferences = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE)
        return sharedPreferences.getBoolean("notificationsEnabled", true)
    }

    fun saveLocationSharing(context: Context, enabled: Boolean){
        val sharedPreferences = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE)
        sharedPreferences.edit().putBoolean("locationSharing", enabled).apply()
    }

    fun getLocationSharing(context: Context): Boolean{
        val sharedPreferences = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE)
        return sharedPreferences.getBoolean("locationSharing", true)
    }

    fun saveLoginCount(context: Context, count: Int){
        val sharedPreferences = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE)
        sharedPreferences.edit().putInt("loginCount", count).apply()
    }

    fun getLoginCount(context: Context): Int{
        val sharedPreferences = context.getSharedPreferences(PREFERENCES_NAME, Context.MODE_PRIVATE)
        return sharedPreferences.getInt("loginCount", 0)
    }

}