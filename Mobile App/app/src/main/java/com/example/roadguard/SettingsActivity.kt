package com.example.roadguard

import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatDelegate
import androidx.appcompat.widget.AppCompatButton
import androidx.preference.*
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.databinding.ActivitySettingsBinding
import com.example.roadguard.sharedPrefs.SharedPrefsHelper
import org.json.JSONObject

class SettingsActivity : BaseActivity() {

    private lateinit var binding: ActivitySettingsBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivitySettingsBinding.inflate(layoutInflater)
        setContentView(binding.root)

        initSlideUpMenu()
        setOutsideTouchListener(R.id.settings_activity)

        supportFragmentManager
            .beginTransaction()
            .replace(R.id.settings_container, SettingsFragment())
            .commit()

    }

    class SettingsFragment : PreferenceFragmentCompat(), Preference.OnPreferenceChangeListener,
        ResponseCallback {
        override fun onCreatePreferences(savedInstanceState: Bundle?, rootKey: String?) {
            setPreferencesFromResource(R.xml.preferences, rootKey)

            val darkModePreference: SwitchPreferenceCompat? = findPreference("dark_mode")
            darkModePreference?.onPreferenceChangeListener = this
        }

        override fun onPreferenceChange(preference: Preference, newValue: Any?): Boolean {
            if (preference.key == "dark_mode") {
                if (newValue is Boolean) {
                    setAppDarkMode(newValue)
                }
            }
            return true
        }

        private fun setAppDarkMode(isDarkModeEnabled: Boolean) {
            if (isDarkModeEnabled) {
                AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_YES)
            } else {
                AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO)
            }
            activity?.recreate()
        }
        private fun loadSettings() {
            val token = SharedPrefsHelper.getToken(requireContext()).toString()
            val client = HTTPRequest()

            client.get(
                requireContext(),
                "${client.clientLink}user/getsettings",
                null,
                this,
                token
            )
        }

        private fun setSwitchPreference(key: String, value: Boolean) {
            val preference = findPreference<SwitchPreferenceCompat>(key)
            preference?.isChecked = value
        }

        private fun setSeekBarPreference(key: String, value: Int) {
            val preference = findPreference<SeekBarPreference>(key)
            preference?.value = value
        }

        private fun setListPreference(key: String, value: String?) {
            val preference = findPreference<ListPreference>(key)
            preference?.value = value
        }

        private fun applySettings(settingsJson: JSONObject) {
            val expectedKeys = setOf(
                "username", "email", "phoneNumber", "firstName", "lastName",
                "locationSharing", "darkMode", "notificationsEnabled",
                "twoFactorAuthEnabled", "alertVolume", "alertType"
            )

            settingsJson.keys().forEach { key ->
                if (key in expectedKeys) {
                    when (key) {
                        "twoFactorAuthEnabled" -> setSwitchPreference("two_factor_auth_enabled", settingsJson.getBoolean(key))
                        "darkMode" -> setSwitchPreference("dark_mode", settingsJson.getBoolean(key))
                        "notificationsEnabled" -> setSwitchPreference("notifications_enabled", settingsJson.getBoolean(key))
                        "alertType" -> setListPreference("alert_type", settingsJson.getString(key))
                        "alertVolume" -> setSeekBarPreference("alert_volume", settingsJson.getInt(key))
                        "locationSharing" -> setSwitchPreference("location_sharing", settingsJson.getBoolean(key))
                        else -> {
                            val preference = findPreference<EditTextPreference>(key)
                            preference?.text = if (settingsJson.isNull(key)) "" else settingsJson.getString(key)
                        }
                    }
                }
            }
        }

        private fun saveSettings() {
            val keys = listOf("username", "email", "phone_number", "first_name", "last_name")
            val jsonKeys = listOf(
                "twoFactorAuthEnabled",
                "darkMode",
                "notificationsEnabled",
                "alertType",
                "alertVolume",
                "locationSharing"
            )

            val settingsJson = JSONObject()

            keys.forEach { key ->
                val value = getEditTextPreference(key)
                settingsJson.put(key, if (value.isNullOrEmpty()) JSONObject.NULL else value)
            }

            jsonKeys.forEach { key ->
                settingsJson.put(
                    key, when (key) {
                        "twoFactorAuthEnabled" -> getSwitchPreference("two_factor_auth_enabled")
                        "darkMode" -> getSwitchPreference("dark_mode")
                        "notificationsEnabled" -> getSwitchPreference("notifications_enabled")
                        "alertType" -> getListPreference("alert_type")
                        "alertVolume" -> getSeekBarPreference("alert_volume")
                        "locationSharing" -> getSwitchPreference("location_sharing")
                        else -> JSONObject.NULL
                    }
                )
            }

             val token = SharedPrefsHelper.getToken(requireContext()).toString()
             val client = HTTPRequest()

            client.post(
                requireContext(),
                "${client.clientLink}user/updatesettings",
                settingsJson.toString(),
                this,
                null,
                token
            )

        }

        private fun getSwitchPreference(key: String): Boolean {
            val preference = findPreference<SwitchPreferenceCompat>(key)
            return preference?.isChecked ?: false
        }

        private fun getSeekBarPreference(key: String): Int {
            val preference = findPreference<SeekBarPreference>(key)
            return preference?.value ?: 0
        }

        private fun getListPreference(key: String): String? {
            val preference = findPreference<ListPreference>(key)
            return preference?.value
        }

        private fun getEditTextPreference(key: String): String? {
            val preference = findPreference<EditTextPreference>(key)
            println("Key: $key, Preference: $preference, Value: ${preference?.text}")
            return preference?.text
        }

        override fun onSuccess(response: String) {
            val responseObject = JSONObject(response)

            when {
                responseObject.keys().next() == "content-type" -> {
                    val dataObject = responseObject.getJSONObject("value").getJSONObject("data")
                    val settingsJson = dataObject.getJSONObject("result")
                    requireActivity().runOnUiThread {
                        applySettings(settingsJson)
                    }
                }
                responseObject.keys().next() == "status" -> {
                    val message = responseObject.getString("message")
                    println(message)
                }
            }
        }


        override fun onFailure(error: Throwable) {
            error.printStackTrace()
        }

        override fun onResume() {
            super.onResume()
            loadSettings()
        }

        override fun onPause() {
            super.onPause()
            saveSettings()
        }
    }
}

