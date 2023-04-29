package com.example.roadguard

import DataStoreHelper
import RoadGuardeesAdapter
import android.content.Intent
import android.os.Bundle
import androidx.appcompat.widget.AppCompatButton
import androidx.lifecycle.lifecycleScope
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.databinding.ActivityRoadguardeesBinding
import kotlinx.coroutines.launch
import org.json.JSONObject

class RoadguardeesActivity : BaseActivity(), ResponseCallback {

    private lateinit var emergencyContactsAdapter: RoadGuardeesAdapter
    private val client = HTTPRequest()

    private lateinit var binding: ActivityRoadguardeesBinding

    private lateinit var myEmergencyContacts: AppCompatButton
    private lateinit var contactList: RecyclerView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityRoadguardeesBinding.inflate(layoutInflater)
        setContentView(binding.root)

        initNavBar()

        contactList = findViewById(R.id.contact_list)
        contactList.layoutManager = LinearLayoutManager(this)
        emergencyContactsAdapter = RoadGuardeesAdapter()
        contactList.adapter = emergencyContactsAdapter

        myEmergencyContacts = findViewById(R.id.btn_my_emergency_contacts)

        myEmergencyContacts.setOnClickListener {
            startActivity(Intent(this, MyRoadguardsActivity::class.java))
        }

        loadEmergencyContacts()
    }

    private fun loadEmergencyContacts() {
        lifecycleScope.launch {
            DataStoreHelper.getToken(this@RoadguardeesActivity).collect { token ->
                client.get(this@RoadguardeesActivity, "${client.clientLink}emergencycontact/getroadguardees", null, this@RoadguardeesActivity, token)
            }
        }
    }

    override fun onSuccess(response: String) {
        val res = JSONObject(response)
        println(res)
        val contacts = res.getJSONObject("value").getJSONArray("data")
        runOnUiThread {
            emergencyContactsAdapter.updateContacts(contacts)
        }

    }

    override fun onFailure(error: Throwable) {
        error.printStackTrace()
    }
}
