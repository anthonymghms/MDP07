package com.example.loginsignup

interface ResponseCallback {
    fun onSuccess(response: String)
    fun onFailure(error: Throwable)
}