package com.example.roadguard

interface ResponseCallback {
    fun onSuccess(response: String)
    fun onFailure(error: Throwable)
}