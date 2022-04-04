import { Form, Button, Dropdown, DropdownButton } from 'react-bootstrap';
import { useState, useEffect } from 'react';

const Lobby = ({ connection, setIsValid, setUserName, setLoginMessage, setActiveRoom, userName, availableRooms, loginMessage }) => {
const [room, setRoom] = useState(null);
const [newRoom, setNewRoom] = useState("");
const [isActive, setIsActive] = useState(false);
const [isUpdatePasswordActive, setIsUpdatePasswordActive] = useState(false);
const [newPassword, setNewPassword] = useState("");

useEffect(() => {
  if (room === null)
    return setIsActive(false)
  if (room === "Custom/New" && newRoom === "")
    return setIsActive(false)
  setIsActive(true)
}, [room, newRoom])

const joinRoom = async (param) => {
  //try {
    await connection.invoke("JoinRoom", param )
    setActiveRoom(param.activeroom);
  //} catch (error) {
  //  console.log(error);
  //}
}

const logout = () => {
  setIsValid(false)
  setUserName('')
  setLoginMessage('')
}

const handleSelect = (value) => (
  setNewRoom(""),
  setRoom(value)
)

const handleCustomNew = (e) => setNewRoom(e.target.value)

const handleIsUpdatePasswordToggle = (e) => {
  e.preventDefault()
  setIsUpdatePasswordActive(!isUpdatePasswordActive);
}

const handleUpdatePasswordInput = (e) => {
  setNewPassword(e.target.value)
  }
  
const handleUpdatePasswordSubmit = async (e) => {
  e.preventDefault();
  var param = {
    username : userName,
    password : newPassword
  }
  await connection.invoke("UpdatePassword", param)

}

  return (
    <div className="col-4 align-self-center">
    <h2>{loginMessage}</h2>
    <Form className='lobby'
    onSubmit={ e => {
      e.preventDefault();
      var param = {
        username: userName,
        activeroom: newRoom === "" ? room : newRoom
      }
      joinRoom(param);
    }}>

      <div className="d-grid gap-2">
      <DropdownButton title={room?room:"Select Room"} 
      onSelect={handleSelect} 
      >
        {availableRooms===null ? <></> : availableRooms.map((room) => (
          <Dropdown.Item eventKey={room}>{room}</Dropdown.Item>
        ))}
      </DropdownButton>
      {room==="Custom/New"
      ?<Form.Control
          placeholder="Room Name..." 
          onChange={handleCustomNew}/>
      :<></>}
      
      <Button
      variant='success' 
      type='submit' 
      disabled={!isActive}
      >Join</Button>

      <Button
      variant='info' 
      onClick={handleIsUpdatePasswordToggle}
      type='button' 
      >Update Password</Button>

      {isUpdatePasswordActive
      ?<><Form.Control
          placeholder="New Password" 
          onChange={handleUpdatePasswordInput}/>
          
          <Button
          variant='dark' 
          onClick={handleUpdatePasswordSubmit}
          type='button' 
          >Submit New Password</Button></>
      :<></>}

      <Button
      variant='danger'
      onClick={logout} 
      type='button' 
      // disabled={!isActive}
      >Log Out</Button>
      </div>
    </Form>
    </div>
  )
}

export default Lobby;