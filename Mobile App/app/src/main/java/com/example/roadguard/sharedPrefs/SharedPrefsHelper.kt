package com.example.roadguard.sharedPrefs

import android.content.Context


object SharedPrefsHelper {
    private const val PREFRENCES_NAME = "RoadGuardPrefs"
    private const val TOKEN_KEY = "AuthToken"

    fun saveToken(context: Context, token: String) {
        val sharedPreferences = context.getSharedPreferences(PREFRENCES_NAME, Context.MODE_PRIVATE)
        sharedPreferences.edit().putString(TOKEN_KEY, token).apply()
    }

    fun getToken(context: Context): String? {
        val sharedPreferences = context.getSharedPreferences(PREFRENCES_NAME, Context.MODE_PRIVATE)
        return sharedPreferences.getString(TOKEN_KEY, null)
    }
}