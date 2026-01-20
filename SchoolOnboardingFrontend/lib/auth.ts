export type UserRole = 'eo' | 'hr' | 'it'

interface AuthState {
  role: UserRole | null
  userId: string | null
}

let authState: AuthState = {
  role: null,
  userId: null,
}

export const useAuth = () => {
  return {
    ...authState,
    setAuth: (role: UserRole, userId: string) => {
      authState.role = role
      authState.userId = userId
    },
    logout: () => {
      authState.role = null
      authState.userId = null
    },
  }
}
