package com.example.roadguard

import android.os.Bundle
import androidx.appcompat.app.AppCompatDelegate
import androidx.preference.Preference
import androidx.preference.PreferenceFragmentCompat
import androidx.preference.SwitchPreferenceCompat
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.databinding.ActivitySettingsBinding

class SettingsActivity : BaseActivity(), ResponseCallback {

    private lateinit var binding: ActivitySettingsBinding
    private var client: HTTPRequest = HTTPRequest()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivitySettingsBinding.inflate(layoutInflater)
        setContentView(binding.root)
        initSlideUpMenu()
        setOutsideTouchListener(R.id.settings_activity)
        getSettings()

        supportFragmentManager
            .beginTransaction()
            .replace(R.id.settings_container, SettingsFragment())
            .commit()
    }

    class SettingsFragment : PreferenceFragmentCompat(), Preference.OnPreferenceChangeListener {
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


    }

    fun setSettings(){

    }

    fun getSettings(){

    }

    override fun onSuccess(response: String) {
        TODO("Not yet implemented")
    }

    override fun onFailure(error: Throwable) {
        TODO("Not yet implemented")
    }
}
