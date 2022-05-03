import { useParams } from "react-router-dom";
import { Form, Button } from 'react-bootstrap';
import { useState } from "react";

const User_CreateGuest = ({ connection }) => {
  
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [loginType, setLoginType] = useState("Guest")

  const sendLoginAttempt = () => {
    const user = {
      Username: username,
      Password: password,
      LoginType: loginType
    }
    console.log("ReturnLoginAttempt:", user)
    connection.send("ReturnLoginAttempt", user)
  }

    return(
      <>
      <Form.Group>
              <Form.Control 
                placeholder="Username..." 
                onChange={e => setUsername(e.target.value)} 
                />
            </Form.Group>
        
            <Button
              variant='success' 
              type='submit'
              onClick={sendLoginAttempt} 
              disabled={!username}
            >Continue as Guest</Button>
      </>
    )
  }
  
  export default User_CreateGuest;