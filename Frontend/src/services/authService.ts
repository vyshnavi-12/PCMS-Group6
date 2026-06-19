import axios from "axios";

const API = axios.create({
  baseURL: "https://localhost:7119/api",
  withCredentials: true
});

export const loginUser = async (email: string, password: string) => {
  const response = await API.post("/user/login", {
    email,
    password
  });

  return response.data;
};

export const getMe = async () => {
  const response = await API.get("/user/me");
  return response.data;
};

export const logoutUser = async () => {
  const response = await API.post("/user/logout");
  return response.data;
};

export default API;