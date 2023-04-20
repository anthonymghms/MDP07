package com.example.roadguard

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import org.json.JSONArray

class EmergencyContactAdapter (
    private var emergencyContacts: JSONArray = JSONArray()
    ): RecyclerView.Adapter<EmergencyContactAdapter.ViewHolder>() {
        class ViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
            val contactName: TextView = itemView.findViewById(R.id.contact_name)
            val contactNumber: TextView = itemView.findViewById(R.id.contact_phone_number)
        }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.emergency_contact_item, parent, false)
        return ViewHolder(view)
    }

    override fun getItemCount() = emergencyContacts.length()

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val contact = emergencyContacts.getJSONObject(position)
        holder.contactName.text = contact.getString("name")
        holder.contactNumber.text = contact.getString("phoneNumber")
    }

    private var contacts = mutableListOf<Pair<String, String>>()

    fun updateContacts(newContacts: List<Pair<String, String>>) {
        contacts.clear()
        contacts.addAll(newContacts)
        notifyDataSetChanged()
    }


}
