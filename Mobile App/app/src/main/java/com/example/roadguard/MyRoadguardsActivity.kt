package com.example.roadguard

import DataStoreHelper
import MyRoadGuardAdapter
import SearchRoadGuardAdapter
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.LayoutInflater
import android.widget.EditText
import androidx.appcompat.app.AlertDialog
import androidx.lifecycle.lifecycleScope
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.roadguard.client.HTTPRequest
import com.example.roadguard.client.ResponseCallback
import com.example.roadguard.databinding.ActivityMyRoadguardsBinding
import com.google.android.material.floatingactionbutton.FloatingActionButton
import kotlinx.coroutines.launch
import org.json.JSONObject

class MyRoadguardsActivity : BaseActivity(), ResponseCallback {

    private lateinit var myRoadGuardAdapter: MyRoadGuardAdapter
    private val client = HTTPRequest()

    private lateinit var binding: ActivityMyRoadguardsBinding

    private lateinit var contactList: RecyclerView

    private lateinit var addFab: FloatingActionButton

    private lateinit var alertDialog: AlertDialog
    lateinit var searchedUsersRecyclerView: RecyclerView
    private lateinit var searchedUserAdapter: SearchRoadGuardAdapter



    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityMyRoadguardsBinding.inflate(layoutInflater)
        setContentView(binding.root)

        initNavBar()

        contactList = findViewById(R.id.my_emergency_contact_list)
        contactList.layoutManager = LinearLayoutManager(this)
        myRoadGuardAdapter = MyRoadGuardAdapter{user ->
            deleteUser(user)
        }
        contactList.adapter = myRoadGuardAdapter

        addFab = findViewById(R.id.add_fab)

        addFab.setOnClickListener {
            showInputBox()
        }

        loadEmergencyContacts()
    }

    private fun deleteUser(user: JSONObject){
        val userName = user.getString("userName")
        lifecycleScope.launch {
            DataStoreHelper.getToken(this@MyRoadguardsActivity).collect{ token ->
                client.post(this@MyRoadguardsActivity,"${client.clientLink}emergencycontact/remove","{}",this@MyRoadguardsActivity,mapOf("ecUsername" to userName),token)
            }
        }
    }

    private fun showInputBox() {
        val inputBoxView = LayoutInflater.from(this).inflate(R.layout.input_layout_box, null)

        searchedUsersRecyclerView = inputBoxView.findViewById(R.id.searched_users_recycler_view)
        searchedUsersRecyclerView.layoutManager = LinearLayoutManager(this)
        searchedUserAdapter = SearchRoadGuardAdapter(this){ searchedUser ->
            addUser(searchedUser)
        }
        searchedUsersRecyclerView.adapter = searchedUserAdapter


        alertDialog = AlertDialog.Builder(this)
            .setTitle("Add RoadGuard")
            .setView(inputBoxView)
            .setPositiveButton("Search", null)
            .setNegativeButton("Close", null)
            .create()

        alertDialog.show()

        alertDialog.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener {
            val inputBox = inputBoxView.findViewById<EditText>(R.id.input_box)
            val inputValue = inputBox.text.toString()

            searchUser(inputValue)
        }
    }



    private fun loadEmergencyContacts() {
        lifecycleScope.launch {
            DataStoreHelper.getToken(this@MyRoadguardsActivity).collect{ token ->
                client.get(this@MyRoadguardsActivity,"${client.clientLink}emergencycontact/getroadguards", null,this@MyRoadguardsActivity,token)
            }
        }

    }

    private fun searchUser(user: String){
        lifecycleScope.launch {
            DataStoreHelper.getToken(this@MyRoadguardsActivity).collect{ token ->
                client.get(this@MyRoadguardsActivity,"${client.clientLink}emergencycontact/getuser", mapOf("search" to user),this@MyRoadguardsActivity,token)
            }
        }
    }

    private fun addUser(user: JSONObject) {
        val userName = user.getString("userName")
        lifecycleScope.launch {
            DataStoreHelper.getToken(this@MyRoadguardsActivity).collect{ token ->
                client.post(this@MyRoadguardsActivity,"${client.clientLink}emergencycontact/add","{}",this@MyRoadguardsActivity,mapOf("ecUsername" to userName),token)
            }
        }
    }

    override fun onSuccess(response: String) {
        val message = JSONObject(JSONObject(response).getString("value")).getString("message")
        println(message)
        when (message) {
            "Emergency Contacts Retrieved Successfully" -> {
                val contacts = JSONObject(response).getJSONObject("value").getJSONArray("data")
                runOnUiThread{
                    myRoadGuardAdapter.updateContacts(contacts)
                }
            }
            "User Retrieved Successfully" -> {
                val contacts = JSONObject(response).getJSONObject("value").getJSONObject("data")
                println(contacts)
                runOnUiThread{
                    searchedUserAdapter.updateContacts(contacts)
                }
            }
            "Emergency Contact Removed Successfully" -> {
                loadEmergencyContacts()
            }
            "Emergency Contact Added Successfully" -> {
                runOnUiThread {
                    val viewHolder = searchedUsersRecyclerView.findViewHolderForAdapterPosition(0) as SearchRoadGuardAdapter.ViewHolder
                    searchedUserAdapter.animateAddButtonForContact(viewHolder, R.drawable.baseline_check_24)
                }
                loadEmergencyContacts()
                Handler(Looper.getMainLooper()).postDelayed({
                    alertDialog.dismiss()
                }, 2500)
            }
        }
    }

    override fun onFailure(error: Throwable) {
        error.printStackTrace()
    }
}
