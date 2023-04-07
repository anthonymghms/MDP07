package com.example.roadguard.client

interface ResponseCallback {
    fun onSuccess(response: String)
    fun onFailure(error: Throwable)
}