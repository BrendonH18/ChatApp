import { useParams } from "react-router-dom";
import { Form, Button } from 'react-bootstrap';
import { useState } from "react";

const User_UpdatePassword = ({ connection }) => {
  
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')

  const sendLoginAttempt = () => {
    const user = {
      Username: username,
      Password: password,
      newPassword: newPassword,
      LoginType: "Update"
    }
    connection.send("ReturnLoginAttempt", user)
  }

    return(
      <>
      <Form.Group>
            <Form.Control 
              placeholder="Username..." 
              onChange={e => setUsername(e.target.value)} />
            <Form.Control
              placeholder="Password..." 
              onChange={e => setPassword(e.target.value)} />
            <Form.Control
              placeholder="New Password..." 
              onChange={e => setNewPassword(e.target.value)} />
          </Form.Group>
      
          <Button
            variant='success' 
            type='submit'
            onClick={sendLoginAttempt} 
            disabled={!username || !password}
          >Update User Password</Button>
      </>
    )
  }
  
  export default User_UpdatePassword;