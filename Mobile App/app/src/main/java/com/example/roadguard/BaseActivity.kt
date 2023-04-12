package com.example.roadguard

import android.content.Intent
import android.os.Bundle
import android.view.MotionEvent
import android.view.View
import android.widget.ImageView
import android.widget.LinearLayout
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.AppCompatButton
import com.google.android.material.bottomsheet.BottomSheetBehavior

open class BaseActivity : AppCompatActivity() {
    private lateinit var bottomSheetBehavior: BottomSheetBehavior<LinearLayout>
    protected lateinit var slideUpArrow: ImageView
    private lateinit var slideUpMenu: LinearLayout
    private lateinit var settingsBtn: AppCompatButton

    protected fun initSlideUpMenu() {
        slideUpMenu = findViewById(R.id.slide_up_menu_container)
        slideUpArrow = findViewById(R.id.slide_up_arrow)
        settingsBtn = findViewById(R.id.settings_button)


        bottomSheetBehavior = BottomSheetBehavior.from(slideUpMenu)
        bottomSheetBehavior.state = BottomSheetBehavior.STATE_COLLAPSED
        bottomSheetBehavior.peekHeight = 0

        slideUpArrow.setOnClickListener {
            if (bottomSheetBehavior.state == BottomSheetBehavior.STATE_COLLAPSED) {
                bottomSheetBehavior.state = BottomSheetBehavior.STATE_EXPANDED
            } else {
                bottomSheetBehavior.state = BottomSheetBehavior.STATE_COLLAPSED
            }
        }

        bottomSheetBehavior.addBottomSheetCallback(object : BottomSheetBehavior.BottomSheetCallback() {
            override fun onStateChanged(bottomSheet: View, newState: Int) {
            }

            override fun onSlide(bottomSheet: View, slideOffset: Float) {
                slideUpArrow.rotation = slideOffset * 180
            }
        })

        settingsBtn.setOnClickListener {
            startActivity(SettingsActivity::class.java)
        }
    }

    protected fun setOutsideTouchListener(containerId: Int) {
        val mainContainer = findViewById<View>(containerId)
        mainContainer.setOnTouchListener { v, event ->
            if (bottomSheetBehavior.state == BottomSheetBehavior.STATE_EXPANDED) {
                val isOutsideTouch =
                    event?.y?.let { it < slideUpMenu.top || it > slideUpMenu.bottom } ?: false
                if (isOutsideTouch) {
                    bottomSheetBehavior.state = BottomSheetBehavior.STATE_COLLAPSED
                    v?.performClick()
                }
            }
            true
        }
    }

    private fun <T> startActivity(activity: Class<T>) {
        val intent = Intent(this, activity)
        startActivity(intent)
    }
}