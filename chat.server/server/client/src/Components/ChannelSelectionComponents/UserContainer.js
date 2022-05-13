const UserContainer = ({ connectedUsers }) => {
  
  const helper = (param) => {
    if (param === null) return ""
    return param.user.username
  }

  return(
    <div className='user-container d-flex flex-column align-items-center'>
      <h4>Connected Users</h4>
      {connectedUsers.map((user, index) => {
        return <h6 key={index}>{helper(user)}</h6> 
      }) }
    </div>
  )
}

export default UserContainer; 