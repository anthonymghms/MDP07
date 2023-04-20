package com.example.roadguard

import android.os.Bundle
import androidx.appcompat.widget.AppCompatButton
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.databinding.ActivityEmergencyContactsBinding
import org.json.JSONArray

class EmergencyContactsActivity : BaseActivity(), ResponseCallback {

    private lateinit var emergencyContactsAdapter: EmergencyContactAdapter
    private val client = HTTPRequest()

    private lateinit var binding: ActivityEmergencyContactsBinding

    private lateinit var addBtn: AppCompatButton
    private lateinit var editBtn: AppCompatButton
    private lateinit var contactList: RecyclerView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityEmergencyContactsBinding.inflate(layoutInflater)
        setContentView(binding.root)

        initSlideUpMenu()
        setOutsideTouchListener(R.id.emergency_contacts_activity)

        contactList = findViewById(R.id.contact_list)
        contactList.layoutManager = LinearLayoutManager(this)
        emergencyContactsAdapter = EmergencyContactAdapter()
        contactList.adapter = emergencyContactsAdapter

        addBtn = findViewById(R.id.add_contact_button)
        editBtn = findViewById(R.id.edit_contact_button)

        addBtn.setOnClickListener {
        }

        editBtn.setOnClickListener {
        }

        loadEmergencyContacts()
    }

    private fun loadEmergencyContacts() {
        val userId = "1"
        val url = "https://example.com/api/emergency_contacts?userId=$userId"
        client.get(this, url, null, this)
    }

    override fun onSuccess(response: String) {
        val jsonArray = JSONArray(response)
        val contacts = mutableListOf<Pair<String, String>>()
        for (i in 0 until jsonArray.length()) {
            val jsonObject = jsonArray.getJSONObject(i)
            val contact = Pair(
                jsonObject.getString("name"),
                jsonObject.getString("phoneNumber")
            )
            contacts.add(contact)
        }
        emergencyContactsAdapter.updateContacts(contacts)
    }

    override fun onFailure(error: Throwable) {
        error.printStackTrace()
    }
}
