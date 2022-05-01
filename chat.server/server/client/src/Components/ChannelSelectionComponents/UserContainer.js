const UserContainer = ({ connectedUsers }) => {
  
  return(
    <div className='user-container d-flex flex-column align-items-center'>
      <h4>Connected Users</h4>
      {connectedUsers.map((user, index) => {
        return <h6 key={index}>{user.username}</h6> 
      })}
    </div>
  )
}

export default UserContainer;