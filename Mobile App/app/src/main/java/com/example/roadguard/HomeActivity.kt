package com.example.roadguard


import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.View
import android.widget.LinearLayout
import com.google.android.material.bottomsheet.BottomSheetBehavior
import androidx.appcompat.app.AlertDialog
import com.example.roadguard.databinding.ActivityHomeBinding

class HomeActivity : AppCompatActivity() {

    private lateinit var bottomSheetBehavior: BottomSheetBehavior<LinearLayout>
    private lateinit var binding: ActivityHomeBinding
    private fun userHasChosenTwoFactorAuth(): Boolean = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)

        if (!userHasChosenTwoFactorAuth()) {
            showTwoFactorAuthDialog()
        }

        bottomSheetBehavior = BottomSheetBehavior.from(binding.slideUpMenuContainer)
        bottomSheetBehavior.state = BottomSheetBehavior.STATE_COLLAPSED
        bottomSheetBehavior.peekHeight = 0

        binding.slideUpArrow.setOnClickListener {
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
                binding.slideUpArrow.rotation = slideOffset * 180
            }
        })

    }


    private fun showTwoFactorAuthDialog() {
        val builder = AlertDialog.Builder(this, R.style.AlertDialogCustom)
        val inflater = layoutInflater
        val dialogView = inflater.inflate(R.layout.dialog_two_factor_auth, null)

        builder.setView(dialogView)
        builder.setPositiveButton("Enable") { dialog, _ ->
            dialog.dismiss()
        }

        builder.setNegativeButton("No, thank you") { dialog, _ ->
            dialog.dismiss()
        }

        val alertDialog = builder.create()
        alertDialog.show()
    }


}