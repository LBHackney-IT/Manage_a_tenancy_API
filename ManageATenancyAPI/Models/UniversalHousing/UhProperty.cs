﻿using Dapper.Contrib.Extensions;

namespace ManageATenancyAPI.Models.UniversalHousing
{
    [Table("property")]
    public class UhProperty
    {
        [Key]
        public string prop_ref { get; set; }
        public string level_code { get; set; }
        public string major_ref { get; set; }
        public string short_address { get; set; }
        public string agent { get; set; }

        #region unused columns 

        //public string man_scheme { get; set; }
        //public string post_code { get; set; }
        //public string post_desig { get; set; }
        //public string telephone { get; set; }
        //public string managed_property { get; set; }
        //public string ownership { get; set; }
        //public string comments { get; set; }
        //public string housing_officer { get; set; }
        //public string area_office { get; set; }
        //public string subtyp_code { get; set; }
        //public string condition_code { get; set; }
        //public string warden_ref { get; set; }
        //public string la_ref { get; set; }
        //public string water_ref { get; set; }
        //public string scheme_ref { get; set; }
        //public string insur_policy { get; set; }
        //public string letable { get; set; }
        //public string practical_completion { get; set; }
        //public string handover { get; set; }
        //public string cat_type { get; set; }
        //public string lounge { get; set; }
        //public string laundry { get; set; }
        //public string visitor_bed { get; set; }
        //public string store { get; set; }
        //public string warden_flat { get; set; }
        //public string sheltered { get; set; }
        //public string house_ref { get; set; }
        //public string occ_stat { get; set; }
        //public string cyclical_due { get; set; }
        //public string shower { get; set; }
        //public string heating { get; set; }
        //public string rep_area { get; set; }
        //public string ac_meth { get; set; }
        //public string propsize { get; set; }
        //public string rtb { get; set; }
        //public string ratevalue { get; set; }
        //public string post_preamble { get; set; }
        //public string core_shared { get; set; }
        //public string rep_officer { get; set; }
        //public string ins_value { get; set; }
        //public string u_nom2 { get; set; }
        //public string region { get; set; }
        //public string asbestos { get; set; }
        //public string accomfund { get; set; }
        //public string candsfund { get; set; }
        //public string property_sid { get; set; }
        //public string keys { get; set; }
        //public string company { get; set; }
        //public string lett_area { get; set; }
        //public string rtb_application { get; set; }
        //public string no_maint { get; set; }
        //public string maintresp { get; set; }
        //public string leasehold { get; set; }
        //public string s125 { get; set; }
        //public string planned_repair_area { get; set; }
        //public string lra_ref { get; set; }
        //public string co_code { get; set; }
        //public string rep_subarea { get; set; }
        //public string con_key { get; set; }
        //public string walk_no { get; set; }
        //public string walk_sequence { get; set; }
        //public string tstamp { get; set; }
        //public string alinefull { get; set; }
        //public string arr_patch { get; set; }
        //public string arr_officer { get; set; }
        //public string dh_status { get; set; }
        //public string dh_assdate { get; set; }
        //public string dh_yearfail { get; set; }
        //public string dh_costnow { get; set; }
        //public string dh_costatfail { get; set; }
        //public string sap_rating { get; set; }
        //public string nher_rating { get; set; }
        //public string num_bedrooms { get; set; }
        //public string comm_lifts { get; set; }
        //public string ent_level { get; set; }
        //public string int_floors { get; set; }
        //public string garden_type { get; set; }
        //public string pets_allowed { get; set; }
        //public string parking { get; set; }
        //public string minage_restric { get; set; }
        //public string family_size { get; set; }
        //public string child_allowed { get; set; }
        //public string local_conn { get; set; }
        //public string alloc_panel { get; set; }
        //public string num_steps { get; set; }
        //public string garage { get; set; }
        //public string maxage_restric { get; set; }
        //public string stair_lift { get; set; }
        //public string through_lift { get; set; }
        //public string acc_shower { get; set; }
        //public string ramp { get; set; }
        //public string hand_rails { get; set; }
        //public string dining_room { get; set; }
        //public string kitchen_dining { get; set; }
        //public string sec_toileta { get; set; }
        //public string sec_toiletb { get; set; }
        //public string cooking_fuel { get; set; }
        //public string comp_avail { get; set; }
        //public string comp_display { get; set; }
        //public string no_single_beds { get; set; }
        //public string no_double_beds { get; set; }
        //public string online_repairs { get; set; }
        //public string vm_propref { get; set; }
        //public string voidman_live { get; set; }
        //public string repairable { get; set; }
        //public string address1 { get; set; }
        //public string u_prop_zone { get; set; }
        //public string u_surveyor_patch { get; set; }
        //public string u_estate { get; set; }
        //public string u_block { get; set; }
        //public string u_location { get; set; }
        //public string u_rent_account { get; set; }
        //public string u_floors { get; set; }
        //public string u_living_rooms { get; set; }
        //public string u_access { get; set; }
        //public string u_amarchetype { get; set; }
        //public string u_priority_estate { get; set; }
        //public string u_comm_entry { get; set; }
        //public string u_consult_stat { get; set; }
        //public string u_corr_width { get; set; }
        //public string u_dpa_service { get; set; }
        //public string u_est_quality { get; set; }
        //public string u_est_security { get; set; }
        //public string u_ext_decent { get; set; }
        //public string u_gas_comments { get; set; }
        //public string u_gas_service_req { get; set; }
        //public string u_int_decent { get; set; }
        //public string u_lever_taps { get; set; }
        //public string u_lift_manufact { get; set; }
        //public string u_rtb_start { get; set; }
        //public string u_sold_leased { get; set; }
        //public string u_sold_leased_date { get; set; }
        //public string u_disabled_only { get; set; }
        //public string u_date_disposed_due { get; set; }
        //public string u_leased_from { get; set; }
        //public string u_lease_end_date { get; set; }
        //public string u_estate_management { get; set; }
        //public string u_access_floor { get; set; }
        //public string u_lift_available { get; set; }
        //public string u_block_floors { get; set; }
        //public string u_balcony { get; set; }
        //public string u_door_entry { get; set; }
        //public string u_council_property { get; set; }
        //public string u_oap_only { get; set; }
        //public string u_disabled_occupier { get; set; }
        //public string u_estate_map_ref { get; set; }
        //public string u_plan_type { get; set; }
        //public string u_year_constructed { get; set; }
        //public string u_collection_round { get; set; }
        //public string u_temporary_accom { get; set; }
        //public string u_window_type { get; set; }
        //public string u_quality_index { get; set; }
        //public string u_asbestos_item { get; set; }
        //public string u_disposed_type { get; set; }
        //public string u_rent_subzone { get; set; }
        //public string u_legal_cases { get; set; }
        //public string u_asbestos_date { get; set; }
        //public string u_llpg_ref { get; set; }
        //public string u_lift_type { get; set; }
        //public string u_ghost_block { get; set; }
        //public string u_ghost_address { get; set; }
        //public string u_prop_area_loc { get; set; }
        //public string u_old_finance_code { get; set; }
        //public string u_ha_property { get; set; }
        //public string u_mobility_std { get; set; }
        //public string u_mobility_wchair { get; set; }
        //public string u_no_lifts { get; set; }
        //public string u_northing { get; set; }
        //public string u_overall_decent { get; set; }
        //public string u_prop_sort_key { get; set; }
        //public string u_raised_sockets { get; set; }
        //public string u_ramped_access { get; set; }
        //public string u_stair_lift { get; set; }
        //public string u_wchair_std { get; set; }
        //public string u_kitchenunit { get; set; }
        //public string u_reasondisposed { get; set; }
        //public string u_mraarchetype { get; set; }
        //public string u_assetarchetype { get; set; }
        //public string u_hraarchetype { get; set; }
        //public string u_lsvtarchetype { get; set; }
        //public string u_beaconcodes { get; set; }
        //public string u_llpgref { get; set; }
        //public string u_alarm { get; set; }
        //public string u_cat_type { get; set; }
        //public string u_ceiling_hoist { get; set; }
        //public string u_disabled_extend { get; set; }
        //public string u_dh_ext_prog { get; set; }
        //public string u_dh_int_prog { get; set; }
        //public string u_int_balcony { get; set; }
        //public string u_wchair_int_access { get; set; }
        //public string u_lowered_switches { get; set; }
        //public string u_raised_socket { get; set; }
        //public string u_front_ramp { get; set; }
        //public string u_rear_ramp { get; set; }
        //public string u_scooter_store { get; set; }
        //public string u_stair_lift_type { get; set; }
        //public string u_hand_rail_type { get; set; }
        //public string u_rear_ent_steps { get; set; }
        //public string u_through_lift { get; set; }
        //public string u_no_wcs { get; set; }
        //public string u_wc_closomat { get; set; }
        //public string u_widened_doors { get; set; }
        //public string owner_conf { get; set; }
        //public string epc_cert_no { get; set; }
        //public string epc_cert_date { get; set; }
        //public string epc_surv_date { get; set; }
        //public string epc_rq_date { get; set; }
        //public string epc_rec_date { get; set; }
        //public string epc_energy { get; set; }
        //public string epc_co2 { get; set; }
        //public string sc_sinkfund { get; set; }
        //public string u_s20_factor { get; set; }
        //public string u_buy_back_date { get; set; }
        //public string u_shared_bathroom { get; set; }
        //public string u_shared_toilet { get; set; }
        //public string u_temp_tenure { get; set; }
        //public string u_disrepair { get; set; }
        //public string u_lha_area { get; set; }
        //public string u_est_man { get; set; }
        //public string u_cleaner { get; set; }
        //public string u_ahr_cat { get; set; }
        //public string u_shared_kitchen { get; set; }
        //public string u_rsl_prop_ref { get; set; }
        //public string u_uses_com_boiler { get; set; }
        //public string u_uses_door_ent { get; set; }
        //public string u_uses_lift { get; set; }
        //public string u_mw_patch { get; set; }
        //public string u_year_built { get; set; }
        //public string u_hand_back_date { get; set; }
        //public string u_bedroom_bedsize { get; set; }
        //public string u_mkt_info_online { get; set; }
        //public string u_mkt_info_magazine { get; set; }
        //public string dtstamp { get; set; }
        //public string u_hgas { get; set; }
        //public string u_access_type { get; set; }
        //public string u_storage_space { get; set; }
        //public string u_internal_steps { get; set; }
        //public string u_external_steps { get; set; }
        //public string u_hoists { get; set; }
        //public string u_intercom { get; set; }
        //public string u_adapted_kitchen { get; set; }

        #endregion
    }
}
