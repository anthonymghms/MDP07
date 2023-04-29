package com.example.roadguard

import android.content.Intent
import android.view.View
import android.widget.ImageView
import android.widget.LinearLayout
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.AppCompatButton
import com.google.android.material.bottomnavigation.BottomNavigationView
import com.google.android.material.bottomsheet.BottomSheetBehavior

open class BaseActivity : AppCompatActivity() {

    protected fun initNavBar() {
        val bottomNavigationView = findViewById<BottomNavigationView>(R.id.bottomNav)
        bottomNavigationView.setOnNavigationItemSelectedListener { item ->
            when (item.itemId) {
                R.id.home -> {
                    startActivity(HomeActivity::class.java)
                    true
                }
                R.id.roadguardees -> {
                    startActivity(RoadguardeesActivity::class.java)
                    true
                }
                R.id.settings -> {
                    startActivity(SettingsActivity::class.java)
                    true
                }
                R.id.roadguards -> {
                    startActivity(MyRoadguardsActivity::class.java)
                    true
                }
                else -> false
            }

        }
    }

    private fun <T> startActivity(activity: Class<T>) {
        val intent = Intent(this, activity)
        startActivity(intent)
    }
}