package com.example.roadguard.client

import android.app.Activity
import android.content.Context
import android.os.Handler
import android.os.Looper
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.FrameLayout
import okhttp3.*
import java.io.IOException
import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager
import androidx.appcompat.app.AppCompatActivity
import com.example.roadguard.R


class HTTPRequest : AppCompatActivity() {

    private val privateClient: OkHttpClient = unSafeOkHttpClient().build()
    private var progressBarContainer: FrameLayout? = null

    val clientLink = "https://roadguard.azurewebsites.net/api/"

    private fun showLoader(context: Context) {
        Handler(Looper.getMainLooper()).postDelayed({
            if (progressBarContainer == null) {
                progressBarContainer = LayoutInflater.from(context)
                    .inflate(R.layout.progress_bar_layout, (context as Activity).findViewById(android.R.id.content), false) as FrameLayout
                (context as Activity).addContentView(progressBarContainer, FrameLayout.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT))
            }
            runOnUiThread{
                progressBarContainer?.visibility = View.VISIBLE
            }
        }, 100)
    }


    fun hideLoader() {
        runOnUiThread {
            progressBarContainer?.visibility = View.GONE
        }
    }

    private fun unSafeOkHttpClient() : OkHttpClient.Builder {
        val okHttpClient = OkHttpClient.Builder()
        try {
            val trustAllCerts:  Array<TrustManager> = arrayOf(object : X509TrustManager {
                override fun checkClientTrusted(chain: Array<out X509Certificate>?, authType: String?){}
                override fun checkServerTrusted(chain: Array<out X509Certificate>?, authType: String?) {}
                override fun getAcceptedIssuers(): Array<X509Certificate>  = arrayOf()
            })

            val  sslContext = SSLContext.getInstance("SSL")
            sslContext.init(null, trustAllCerts, SecureRandom())

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

    fun get(context: Context, url: String, queryParams: Map<String, String>? = null, callback: ResponseCallback, token: String? = null) {

        showLoader(context)

        val httpUrl = HttpUrl.parse(url)

        if (httpUrl != null) {
            val httpUrlBuilder = httpUrl.newBuilder()

            if (queryParams != null) {
                for (param in queryParams) {
                    httpUrlBuilder.addQueryParameter(param.key, param.value)
                }
            }

            val requestBuilder = Request.Builder()
                .addHeader("X-Api-Key", "4EBD8459736F407D9697AED213DBDAF6")
                .url(httpUrlBuilder.build())

            token?.let { requestBuilder.addHeader("Authorization", "Bearer $it") }

            val request = requestBuilder.build()

            privateClient.newCall(request).enqueue(object : Callback {
                override fun onFailure(call: Call, e: IOException) {
                    e.printStackTrace()
                    hideLoader()
                }

                override fun onResponse(call: Call, response: Response) {
                    response.body()?.let { responseBody ->
                        val responseString = responseBody.string()
                        Log.d("PostResponse", "Response Code: ${response.code()}")
                        Log.d("PostResponse", "Response Body: $responseString")

                        if (response.isSuccessful || response.code() == 423 || response.code() == 401 || response.code() == 403) {
                            callback.onSuccess(responseString)
                            hideLoader()
                        } else {
                            callback.onFailure(IOException("Unexpected response code: ${response.code()}"))
                            hideLoader()
                        }
                    }
                }

            })
        } else {
            Log.e("GetFunction", "Invalid URL: $url")
            hideLoader()
        }
    }


    fun post(context: Context,url: String, jsonBody: String, callback: ResponseCallback, queryParams: Map<String, String>? = null, token: String? = null) {

        showLoader(context)

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

            val requestBuilder = Request.Builder()
                .addHeader("X-Api-Key", "4EBD8459736F407D9697AED213DBDAF6")
                .addHeader("Content-Type", "application/json")
                .url(httpUrlBuilder.build())
                .post(body)

            token?.let { requestBuilder.addHeader("Authorization", "Bearer $it") }

            val request = requestBuilder.build()

            val client = OkHttpClient()

            client.newCall(request).enqueue(object : Callback {
                override fun onFailure(call: Call, e: IOException) {
                    callback.onFailure(e)
                    hideLoader()
                }

                override fun onResponse(call: Call, response: Response) {
                    if (response.isSuccessful || response.code() == 423 || response.code() == 401 || response.code() == 403 ) {
                        val responseBody = response.body()?.string()
                        responseBody?.let { callback.onSuccess(it) }
                        hideLoader()
                    } else {
                        callback.onFailure(IOException("Unexpected response code: ${response.code()}"))
                        hideLoader()
                    }
                }

            })
        } else {
            Log.e("PostFunction", "Invalid URL: $url")
        }

    }

}