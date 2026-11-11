export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  expiresAt: string;
}

export interface AuthSession {
  username: string;
}

export interface User {
  id: string;
  name: string;
  email: string;
}

export interface TaskStatus {
  id: string;
  name: string;
}

export interface TaskItem {
  id: string;
  title: string;
  userId: string;
  userName: string;
  statusId: string;
  status: string;
  additionalInfo: Record<string, unknown>;
  createdAt: string;
  startedAt: string | null;
  finishedAt: string | null;
}

export interface CreateTaskRequest {
  title: string;
  userId: string;
  additionalInfo: Record<string, unknown>;
}

export interface UpdateTaskStatusRequest {
  status: string;
}

export interface ApiErrorResponse {
  message: string;
  details: string[];
}
