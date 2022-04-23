import { Form, Button, Dropdown, DropdownButton } from 'react-bootstrap';
import { useState, useEffect } from 'react';

const Lobby = ({ connection, setIsValid, setUserName, setLoginMessage, setActiveRoom, userName, availableChannels, loginMessage, loginType}) => {
const [room, setRoom] = useState(null);
const [newRoom, setNewRoom] = useState("");
const [isActive, setIsActive] = useState(false);
const [isUpdatePasswordActive, setIsUpdatePasswordActive] = useState(false);
const [newPassword, setNewPassword] = useState("");
const [isPasswordUpdated, setIsPasswordUpdated] = useState(undefined);

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
    setIsPasswordUpdated(undefined);
  //} catch (error) {
  //  console.log(error);
  //}
}

const logout = () => {
  setIsValid(false)
  setUserName('')
  setLoginMessage('')
}

const handleSelect = (value) => {
  setNewRoom("");
  setRoom(availableChannels[value-1])
}

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
  connection.on("ReturnedPasswordUpdate", (param) => {
    console.log("ReturnedPassword: ", param)
    setIsPasswordUpdated(param)
  })
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
      console.log(param)
      //joinRoom(param);
    }}>

      <div className="d-grid gap-2">
      <DropdownButton title={room ? room.name : "Select Room"} 
      onSelect={
        handleSelect
      }
      >
        {availableChannels===null ? <></> : availableChannels.map((room) => (
          <Dropdown.Item eventKey={room.id}>{room.name}</Dropdown.Item>
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
      hidden={loginType === "Guest"} 
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
          >Submit New Password</Button>
          
          <h2
          hidden={isPasswordUpdated === undefined}>
          {isPasswordUpdated ? "Password Updated Successfully" : "Password Update Failed"}
          </h2>

          {/* {isPasswordUpdated != null && <h2>{isPasswordUpdated ? "Password Updated Successfully" : "Password Update Failed"}</h2>} */}
          </>

          
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