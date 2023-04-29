import android.content.Context
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map

private val Context.dataStore by preferencesDataStore("RoadGuardDataStore")

object DataStoreHelper {

    private val TOKEN_KEY = stringPreferencesKey("AuthToken")

    suspend fun saveToken(context: Context, token: String) {
        context.dataStore.edit { preferences ->
            preferences[TOKEN_KEY] = token
        }
    }

    fun getToken(context: Context): Flow<String?> {
        return context.dataStore.data.map { preferences ->
            preferences[TOKEN_KEY]
        }
    }

    suspend fun clearToken(context: Context) {
        context.dataStore.edit { preferences ->
            preferences.remove(TOKEN_KEY)
        }
    }

    private val KEEP_ME_LOGGED_IN_KEY = stringPreferencesKey("keepMeLoggedIn")

    suspend fun saveKeepMeLoggedIn(context: Context, keepMeLoggedIn: Boolean) {
        context.dataStore.edit { preferences ->
            preferences[KEEP_ME_LOGGED_IN_KEY] = keepMeLoggedIn.toString()
        }
    }

    fun getKeepMeLoggedIn(context: Context): Flow<Boolean> {
        return context.dataStore.data.map { preferences ->
            preferences[KEEP_ME_LOGGED_IN_KEY]?.toBoolean() ?: false
        }
    }

    private val NOTIFICATIONS_ENABLED_KEY = stringPreferencesKey("notificationsEnabled")

    suspend fun saveNotificationSettings(context: Context, enabled: Boolean) {
        context.dataStore.edit { preferences ->
            preferences[NOTIFICATIONS_ENABLED_KEY] = enabled.toString()
        }
    }

    fun getNotificationSettings(context: Context): Flow<Boolean> {
        return context.dataStore.data.map { preferences ->
            preferences[NOTIFICATIONS_ENABLED_KEY]?.toBoolean() ?: true
        }
    }

    private val LOCATION_SHARING_KEY = stringPreferencesKey("locationSharing")

    suspend fun saveLocationSharing(context: Context, enabled: Boolean) {
        context.dataStore.edit { preferences ->
            preferences[LOCATION_SHARING_KEY] = enabled.toString()
        }
    }

    fun getLocationSharing(context: Context): Flow<Boolean> {
        return context.dataStore.data.map { preferences ->
            preferences[LOCATION_SHARING_KEY]?.toBoolean() ?: true
        }
    }

    private val LOGIN_COUNT_KEY = stringPreferencesKey("loginCount")

    suspend fun saveLoginCount(context: Context, count: Int) {
        context.dataStore.edit { preferences ->
            preferences[LOGIN_COUNT_KEY] = count.toString()
        }
    }

    fun getLoginCount(context: Context): Flow<Int> {
        return context.dataStore.data.map { preferences ->
            preferences[LOGIN_COUNT_KEY]?.toInt() ?: 0
        }
    }

    private val APP_THEME_KEY = stringPreferencesKey("appTheme")

    suspend fun saveAppTheme(context: Context, theme: String) {
        context.dataStore.edit { preferences ->
            preferences[APP_THEME_KEY] = theme
        }
    }

    fun getAppTheme(context: Context): Flow<String> {
        return context.dataStore.data.map { preferences ->
            preferences[APP_THEME_KEY] ?: "light"
        }
    }

}
