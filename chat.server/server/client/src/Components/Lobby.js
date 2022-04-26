import { Form, Button, Dropdown, DropdownButton } from 'react-bootstrap';
import { useState, useEffect } from 'react';

const Lobby = ({ connection, setChannel, availableChannels, userConnection }) => {

  const [passwordToggle, setPasswordToggle] = useState(false)
  const [newPassword, setNewPassword] = useState('')
  const [passwordUpdateMessage, setPasswordUpdateMessage] = useState(null)

  const handleJoinChannel = () => {
    connection.send("JoinChannel", userConnection);
  }
  const handleLogOut = () => {
    connection.send("LogOut")
  }
  const handleSelect = (value) => {
    setChannel(availableChannels[value-1])
  }
  const handleNewPasswordSubmit = async (e) => {
    await connection.invoke("UpdatePassword", newPassword)
    connection.on("ReturnedPasswordUpdate", (param) => {
      console.log("ReturnedPassword: ", param)
      setPasswordUpdateMessage(param)
    })
  }

  return (
    <div className="col-4 align-self-center">
    {/* <h2>{loginMessage}</h2> */}
    <Form className='lobby'
    onSubmit={ e => {
      e.preventDefault();
      handleJoinChannel();
    }}>

      <div className="d-grid gap-2">
      <DropdownButton title={ 
        userConnection.Channel.name ? userConnection.Channel.name : "Select Room"} 
      onSelect={ handleSelect}
      >
        {availableChannels===null ? <></> : availableChannels.map((room) => (
          <Dropdown.Item eventKey={room.id}>{room.name}</Dropdown.Item>
        ))}
      </DropdownButton>
      {/* {room==="Custom/New"
      ?<Form.Control
          placeholder="Room Name..." 
          onChange={handleCustomNew}/>
      :<></>} */}
      
      <Button
      variant='success' 
      type='submit' 
      disabled={ userConnection.Channel.name ? false : true }
      >Join</Button>

      <Button
      variant='info'
      hidden={userConnection.User.loginType === "Guest" ? true : false} 
      onClick={e => setPasswordToggle(!passwordToggle)}
      type='button' 
      >Update Password</Button>

      {passwordToggle
      ?<><Form.Control
          placeholder="New Password" 
          onChange={e => setNewPassword(e.target.value)}/>
          
          <Button
          variant='dark' 
          onClick={e => handleNewPasswordSubmit()}
          type='button' 
          >Submit New Password</Button>
          
          <h2
          hidden={passwordUpdateMessage === null}>
          {passwordUpdateMessage}
          </h2>

          </>
      :<></>}

      <Button
      variant='danger'
      onClick={e => handleLogOut()} 
      type='button' 
      >Log Out</Button>
      </div>
    </Form>
    </div>
  )
}

export default Lobby;