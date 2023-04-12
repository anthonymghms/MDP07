package com.example.roadguard

import android.os.Bundle
import androidx.appcompat.app.AlertDialog
import com.example.roadguard.databinding.ActivityHomeBinding

class HomeActivity : BaseActivity() {

    private lateinit var binding: ActivityHomeBinding
    private fun userHasChosenTwoFactorAuth(): Boolean = false
    private var twoFactorAuthDialog: AlertDialog? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityHomeBinding.inflate(layoutInflater)
        setContentView(binding.root)
        initSlideUpMenu()
        setOutsideTouchListener(R.id.home_activity)
        if (!userHasChosenTwoFactorAuth()) {
            showTwoFactorAuthDialog()
        }
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

        twoFactorAuthDialog = builder.create()
        twoFactorAuthDialog?.show()
    }

    override fun onDestroy() {
        twoFactorAuthDialog?.dismiss()
        super.onDestroy()
    }
}