import { useNavigate } from "react-router-dom"
import BubbleLabel from "../../../Formatting/BubbleLabel"

const Body = ({trimChannels, ActiveChannelID, isConnectedUsersByChannelAndStatusTruthy, connectedUsersByChannelAndStatus}) => {
let navigate = useNavigate()

const handleChannelSelect = (channel) => {
    navigate(`/Channel/${channel.id}`)
}

return <>
    
        <ul className="contacts">
            {trimChannels
                ? trimChannels.map(channel => 
                    <li key={`channel-${channel.id}`} className={parseInt(ActiveChannelID)===parseInt(channel.id) ? "active" : ""} onClick={e => handleChannelSelect(channel)}>
                        <BubbleLabel image={channel.image} label={channel.name} subLabel={`${isConnectedUsersByChannelAndStatusTruthy() ? 3 : 0} Connected`} isOnline={isConnectedUsersByChannelAndStatusTruthy()} numberConnectedUsers={isConnectedUsersByChannelAndStatusTruthy() ? 3 : 0} />
                    </li>
                )
                : <></>}
        </ul>
</>
} 

export default Body