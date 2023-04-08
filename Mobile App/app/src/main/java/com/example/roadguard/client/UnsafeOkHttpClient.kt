package com.example.roadguard.client

import android.util.Log
import okhttp3.*
import java.io.IOException
import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager
import androidx.appcompat.app.AppCompatActivity


class HTTPRequest : AppCompatActivity() {

    val privateClient: OkHttpClient = unSafeOkHttpClient().build()

    private fun unSafeOkHttpClient() : OkHttpClient.Builder {
        val okHttpClient = OkHttpClient.Builder()
        try {
            // Create a trust manager that does not validate certificate chains
            val trustAllCerts:  Array<TrustManager> = arrayOf(object : X509TrustManager {
                override fun checkClientTrusted(chain: Array<out X509Certificate>?, authType: String?){}
                override fun checkServerTrusted(chain: Array<out X509Certificate>?, authType: String?) {}
                override fun getAcceptedIssuers(): Array<X509Certificate>  = arrayOf()
            })

            // Install the all-trusting trust manager
            val  sslContext = SSLContext.getInstance("SSL")
            sslContext.init(null, trustAllCerts, SecureRandom())

            // Create an ssl socket factory with our all-trusting manager
            val sslSocketFactory = sslContext.socketFactory
            if (trustAllCerts.isNotEmpty() &&  trustAllCerts.first() is X509TrustManager) {
                okHttpClient.sslSocketFactory(sslSocketFactory, trustAllCerts.first() as X509TrustManager)
                okHttpClient.hostnameVerifier { _, _ -> true }
            }

            return okHttpClient
        } catch (e: Exception) {
            return okHttpClient
        }
    }

    fun get(url: String, queryParams: Map<String, String>? = null , callback: ResponseCallback) {
        val httpUrl = HttpUrl.parse(url)

        if (httpUrl != null) {
            val httpUrlBuilder = httpUrl.newBuilder()

            if (queryParams != null) {
                for (param in queryParams) {
                    httpUrlBuilder.addQueryParameter(param.key, param.value)
                }
            }

            val request = Request.Builder()
                .addHeader("X-Api-Key", "4EBD8459736F407D9697AED213DBDAF6")
                .url(httpUrlBuilder.build())
                .build()

            privateClient.newCall(request).enqueue(object : Callback {
                override fun onFailure(call: Call, e: IOException) {
                    e.printStackTrace()
                }

                override fun onResponse(call: Call, response: Response) {
                    if (response.isSuccessful || response.code() == 423 || response.code() == 401 || response.code() == 403) {
                        response.body()?.string()?.let { callback.onSuccess(it) }
                    } else {
                        callback.onFailure(IOException("Unexpected response code: ${response.code()}"))
                    }
                }            })
        } else {
            Log.e("GetFunction", "Invalid URL: $url")
        }
    }


    fun post(url: String, jsonBody: String, callback: ResponseCallback, queryParams: Map<String, String>? = null) {
        val body: RequestBody = RequestBody.create(
            MediaType.parse("application/json; charset=utf-8"), jsonBody
        )

        val httpUrl = HttpUrl.parse(url)

        if (httpUrl != null) {
            val httpUrlBuilder = httpUrl.newBuilder()

            if (queryParams != null) {
                for (param in queryParams) {
                    httpUrlBuilder.addQueryParameter(param.key, param.value)
                }
            }

            val request = Request.Builder()
                .addHeader("X-Api-Key", "4EBD8459736F407D9697AED213DBDAF6")
                .url(httpUrlBuilder.build())
                .post(body)
                .build()

            val client = OkHttpClient()

            client.newCall(request).enqueue(object : Callback {
                override fun onFailure(call: Call, e: IOException) {
                    callback.onFailure(e)
                }

                override fun onResponse(call: Call, response: Response) {
                    if (response.isSuccessful || response.code() == 423 || response.code() == 401 || response.code() == 403) {
                        response.body()?.string()?.let { callback.onSuccess(it) }
                    } else {
                        callback.onFailure(IOException("Unexpected response code: ${response.code()}"))
                    }
                }
            })
        } else {
            Log.e("PostFunction", "Invalid URL: $url")
        }
    }

}