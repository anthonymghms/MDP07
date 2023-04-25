package com.example.roadguard

import DataStoreHelper
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.appcompat.app.AppCompatDelegate
import androidx.appcompat.widget.AppCompatButton
import androidx.lifecycle.lifecycleScope
import androidx.preference.*
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.databinding.ActivitySettingsBinding
import com.example.roadguard.main.MainActivity
import kotlinx.coroutines.launch
import org.json.JSONObject

class SettingsActivity : BaseActivity() {

    private lateinit var binding: ActivitySettingsBinding

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivitySettingsBinding.inflate(layoutInflater)
        setContentView(binding.root)


        initNavBar()

        supportFragmentManager
            .beginTransaction()
            .replace(R.id.settings_container, SettingsFragment())
            .commit()

    }

    class SettingsFragment : PreferenceFragmentCompat(), Preference.OnPreferenceChangeListener,
        ResponseCallback {

        private lateinit var saveButton: AppCompatButton
        override fun onCreateView(
            inflater: LayoutInflater,
            container: ViewGroup?,
            savedInstanceState: Bundle?
        ): View {
            val view = super.onCreateView(inflater, container, savedInstanceState)
            val activityBinding = ActivitySettingsBinding.bind(requireActivity().findViewById(R.id.settings_activity))
            saveButton = activityBinding.saveButton
            saveButton.setOnClickListener {
                saveSettings()
            }
            return view
        }

        override fun onCreatePreferences(savedInstanceState: Bundle?, rootKey: String?) {
            setPreferencesFromResource(R.xml.preferences, rootKey)

            val darkModePreference: SwitchPreferenceCompat? = findPreference("dark_mode")
            darkModePreference?.onPreferenceChangeListener = this

            val logoutPreference: Preference? = findPreference("logout")
            logoutPreference?.onPreferenceClickListener = Preference.OnPreferenceClickListener {
                performLogout()
                true
            }
        }

        private fun performLogout() {
            lifecycleScope.launch {
                DataStoreHelper.clearToken(requireContext())
                DataStoreHelper.saveKeepMeLoggedIn(requireContext(), false)
            }
            val intent = Intent(requireActivity(), MainActivity::class.java)
            intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK or Intent.FLAG_ACTIVITY_NEW_TASK
            startActivity(intent)
            requireActivity().finish()
        }


        override fun onPreferenceChange(preference: Preference, newValue: Any?): Boolean {
            return true
        }

        private fun setAppDarkMode(isDarkModeEnabled: Boolean) {
            val mode = if (isDarkModeEnabled) AppCompatDelegate.MODE_NIGHT_YES else AppCompatDelegate.MODE_NIGHT_NO
            AppCompatDelegate.setDefaultNightMode(mode)
        }

        private fun loadSettings() {

            val client = HTTPRequest()

            lifecycleScope.launch {
                DataStoreHelper.getToken(requireContext()).collect {token ->
                    client.get(
                        requireContext(),
                        "${client.clientLink}user/getsettings",
                        null,
                        this@SettingsFragment,
                        token
                    )
                }
            }
        }

        private fun setSwitchPreference(key: String, value: Boolean) {
            Log.d("SettingsFragment", "Setting SwitchPreference: Key: $key, Value: $value")
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
                "twoFactorAuthEnabled", "alertVolume", "alertType", "alertLevel","ipCamAddress"
            )

            settingsJson.keys().forEach { key ->
                if (key in expectedKeys) {
                    Log.d("SettingsFragment", "Applying Key: $key, Value: ${settingsJson.get(key)}")
                    when (key) {
                        "twoFactorAuthEnabled" -> setSwitchPreference("two_factor_auth_enabled", settingsJson.getBoolean(key))
                        "darkMode" -> {
                            Log.d("SettingsFragment", "Applying dark mode value: ${settingsJson.getBoolean("darkMode")}")
                            setSwitchPreference("dark_mode", settingsJson.getBoolean(key))
                        }
                        "notificationsEnabled" -> setSwitchPreference("notifications_enabled", settingsJson.getBoolean(key))
                        "alertType" -> setListPreference("alert_type", settingsJson.getString(key))
                        "alertLevel" -> setListPreference("alert_level",settingsJson.getString(key))
                        "alertVolume" -> setSeekBarPreference("alert_volume", settingsJson.getInt(key))
                        "locationSharing" -> setSwitchPreference("location_sharing", settingsJson.getBoolean(key))
                        "ipCamAddress" -> {
                            val preference = findPreference<EditTextPreference>(key)
                            preference?.text = if (settingsJson.isNull(key)) "" else settingsJson.getString(key)
                        }
                        else -> {
                            val preference = findPreference<EditTextPreference>(key)
                            preference?.text = if (settingsJson.isNull(key)) "" else settingsJson.getString(key)
                        }
                    }
                }
            }
        }

        fun saveSettings() {
            val keys = listOf("username", "email", "phone_number", "first_name", "last_name","ipCamAddress")
            val jsonKeys = listOf(
                "twoFactorAuthEnabled",
                "darkMode",
                "notificationsEnabled",
                "alertType",
                "alertVolume",
                "alertLevel",
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
                        "alertLevel" -> getListPreference("alert_level")
                        "alertVolume" -> getSeekBarPreference("alert_volume")
                        "locationSharing" -> getSwitchPreference("location_sharing")
                        else -> JSONObject.NULL
                    }
                )
            }

            Log.d("SettingsFragment", "Sending dark mode value: ${settingsJson.getBoolean("darkMode")}")


            val isDarkMode = settingsJson.getBoolean("darkMode")
            setAppDarkMode(isDarkMode)
            lifecycleScope.launch {
                val appTheme = if (isDarkMode) "dark" else "light"
                DataStoreHelper.saveAppTheme(requireContext(), appTheme)
            }

             val client = HTTPRequest()

            lifecycleScope.launch{
                DataStoreHelper.getToken(requireContext()).collect {token ->
                    client.post(
                        requireContext(),
                        "${client.clientLink}user/updatesettings",
                        settingsJson.toString(),
                        this@SettingsFragment,
                        null,
                        token
                    )
                }
            }
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
            if (response.isNullOrEmpty()) {
                Log.e("SettingsFragment", "Empty response received")
                return
            }

            val responseObject = JSONObject(response)
            Log.d("SettingsFragment", "Response: $responseObject")

            when {
                responseObject.keys().next() == "contentType" -> {
                    val dataObject = responseObject.getJSONObject("value").getJSONObject("data")
                    val settingsJson = dataObject.getJSONObject("result")
                    Log.d("SettingsFragment", "Settings JSON: $settingsJson")
                    Log.d("SettingsFragment", "Received dark mode value: ${settingsJson.getBoolean("darkMode")}")
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

    }
}

