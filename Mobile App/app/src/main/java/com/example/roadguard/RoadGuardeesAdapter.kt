import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.roadguard.R
import org.json.JSONArray

class RoadGuardeesAdapter(
    private var emergencyContacts: JSONArray = JSONArray()
) : RecyclerView.Adapter<RoadGuardeesAdapter.ViewHolder>() {

    class ViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val contactName: TextView = itemView.findViewById(R.id.contact_name)
        val contactNumber: TextView = itemView.findViewById(R.id.contact_phone_number)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.roadguardee_item, parent, false)
        return ViewHolder(view)
    }

    override fun getItemCount() = emergencyContacts.length()

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val contact = emergencyContacts.getJSONObject(position)
        val firstName = contact.getString("firstName")
        val lastName = contact.getString("lastName")
        val fullName = "$firstName $lastName"
        holder.contactName.text = fullName
        holder.contactNumber.text = contact.getString("phoneNumber")
    }

    fun updateContacts(newContacts: JSONArray) {
        emergencyContacts = newContacts
        notifyDataSetChanged()
    }
}
