import { HubConnection } from "@microsoft/signalr";
import { Outlet, useNavigate, useParams } from "react-router-dom";

const ChannelDashboard = ({ availableChannels, setUserConnection, userConnection, connection, setMessages, setConnectedUsers }) => {
  
let navigate = useNavigate()

const handleClick = (e) => {
  // const newUserConnecion = userConnection
  // newUserConnecion.channel = availableChannels[e.target.id - 1]
  // console.log(values)
  // console.log(availableChannels[e.target.id - 1])
  // setConnectedUsers([])
  // setMessages([])
  // connection.send("JoinChannel", availableChannels[e.target.id - 1])
  // setUserConnection(newUserConnecion)
  console.log(availableChannels[e.target.id-1].id)
  navigate(availableChannels[e.target.id - 1].id.toString())
}

    return(
      <>
      {availableChannels.map(x => <button 
        id={x.id}
        onClick={handleClick}
        >
          
          {`${x.name}`}</button>)}
          <Outlet/>
      </>
    )
  }
  
  export default ChannelDashboard;