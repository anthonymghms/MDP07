import android.annotation.SuppressLint
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.animation.Animation
import android.view.animation.AnimationUtils
import android.widget.ImageView
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.roadguard.MyRoadguardsActivity
import com.example.roadguard.R
import org.json.JSONObject

class SearchRoadGuardAdapter(private val myRoadguardsActivity: MyRoadguardsActivity, private val onAddButtonClickListener: (JSONObject) -> Unit)
    : RecyclerView.Adapter<SearchRoadGuardAdapter.ViewHolder>() {

    private var searchedUser: JSONObject? = null

    class ViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val contactName: TextView = itemView.findViewById(R.id.searched_emergency_contact_name)
        val contactNumber: TextView = itemView.findViewById(R.id.searched_emergency_contact_phone_number)
        val addButton: ImageView = itemView.findViewById(R.id.search_add_btn)
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.searched_roadguard_item, parent, false)
        return ViewHolder(view)
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val contact = searchedUser
        if (contact != null) {
            val firstName = contact.getString("firstName")
            val lastName = contact.getString("lastName")
            val fullName = "$firstName $lastName"
            holder.contactName.text = fullName
            holder.contactNumber.text = contact.getString("phoneNumber")
            holder.addButton.setOnClickListener {
                onAddButtonClickListener(contact)
            }
        }
    }

    fun animateAddButtonForContact(holder: ViewHolder, newIcon: Int) {
        val fadeOutAnimation = AnimationUtils.loadAnimation(holder.itemView.context, R.anim.rotate_and_fade_out)
        val fadeInAnimation = AnimationUtils.loadAnimation(holder.itemView.context, R.anim.rotate_and_fade_in)

        fadeOutAnimation.setAnimationListener(object: Animation.AnimationListener {
            override fun onAnimationStart(animation: Animation?) {}

            override fun onAnimationEnd(animation: Animation?) {
                holder.addButton.setImageResource(newIcon)
                holder.addButton.startAnimation(fadeInAnimation)
            }

            override fun onAnimationRepeat(p0: Animation?) {}
        })

        holder.addButton.startAnimation(fadeOutAnimation)
    }




    override fun getItemCount(): Int {
        return if (searchedUser == null) 0 else 1
    }

    @SuppressLint("NotifyDataSetChanged")
    fun updateContacts(newContacts: JSONObject) {
        searchedUser = newContacts
        notifyDataSetChanged()
    }
}
