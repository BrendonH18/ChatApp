import { FontAwesomeIcon } from "@fortawesome/react-fontawesome"
import { faSearch } from "@fortawesome/free-solid-svg-icons"

const SearchBar = ({setSearch, search}) => {

return <>
        <div className="input-group">
            <input type="text" placeholder="Search..." name="" className="form-control search" onChange={e => setSearch(e.target.value)} value={search}/>
            <div className="input-group-prepend">
                <span className="input-group-text search_btn h-100">
                    <FontAwesomeIcon icon={faSearch}/>
                </span>
            </div>
        </div>
</>
}

export default SearchBar