
const UserContainer = ({ users }) => {

  return(
    <div className='user-container d-flex flex-column align-items-center'>
      <h4>Connected Users</h4>
      {users.map((user, index) => {
        return <h6 key={index}>{user}</h6>
      })}
    </div>
  )
}
export default UserContainer;