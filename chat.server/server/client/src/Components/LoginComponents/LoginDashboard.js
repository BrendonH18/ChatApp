import { Form, Button } from 'react-bootstrap';
import { useEffect, useState } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';

const LoginDashboard = ({userConnection, setUserConnection}) => {
  
  const isPasswordValid = () => {
    if (userConnection.user.isPasswordValid) return "visible" 
    return "hidden"
  }
  const navigate = useNavigate()

  return(
    <>
    <h1>Login Dashboard</h1>
    <button onClick={() => navigate("Create")}>Create</button>
    <button onClick={() => navigate("Returning")}>Returning</button>
    <button onClick={() => navigate("Guest")}>Guest</button>
    <button style={{visibility:  isPasswordValid()}} onClick={() => navigate("Update")}>Update Password</button>
    <Outlet/>
    </>
  )
}

export default LoginDashboard