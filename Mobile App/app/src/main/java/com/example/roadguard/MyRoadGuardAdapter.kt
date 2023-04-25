import android.media.Image
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.roadguard.R
import org.json.JSONArray
import org.json.JSONObject

class MyRoadGuardAdapter (
    private var myEmergencyContacts: JSONArray = JSONArray(),
    private val onDeleteButtonClickListener: (JSONObject) -> Unit
) : RecyclerView.Adapter<MyRoadGuardAdapter.ViewHolder>() {

    class ViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val contactName: TextView = itemView.findViewById(R.id.my_emergency_contact_name)
        val contactNumber: TextView = itemView.findViewById(R.id.my_emergency_contact_phone_number)
        val deleteButton: ImageView = itemView.findViewById(R.id.delete_btn)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.roadguards_item, parent, false)
        return ViewHolder(view)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val contact = myEmergencyContacts.getJSONObject(position)
        val firstName = contact.getString("firstName")
        val lastName = contact.getString("lastName")
        val fullName = "$firstName $lastName"
        holder.contactName.text = fullName
        holder.contactNumber.text = contact.getString("phoneNumber")
        holder.deleteButton.setOnClickListener {
            onDeleteButtonClickListener(contact)
        }
    }

    override fun getItemCount() = myEmergencyContacts.length()

    fun updateContacts(newContacts: JSONArray) {
        myEmergencyContacts = newContacts
        notifyDataSetChanged()
    }
}
